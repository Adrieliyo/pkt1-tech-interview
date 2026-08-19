using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShipmentTracker.Core.DTOs.Auth;
using ShipmentTracker.Core.Interfaces;
using ShipmentTracker.Core.Identity;

namespace ShipmentTracker.Web.Controllers
{
    /// <summary>
    /// Autenticación por cookie: login, logout y consulta de la sesión actual.
    /// </summary>
    [Route("api/auth")]
    [ApiController]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<LoginDto> _loginValidator;

        public AuthController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork,
            IValidator<LoginDto> loginValidator)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _loginValidator = loginValidator;
        }

        /// <summary>
        /// Inicia sesión con email y contraseña, emitiendo una cookie de sesión. Endpoint PÚBLICO
        /// (única excepción junto al tracking público, FR-016) — no requiere autenticación previa.
        /// </summary>
        /// <param name="dto">Credenciales de acceso.</param>
        /// <returns>La sesión recién iniciada.</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<UserSessionDto>> Login([FromBody] LoginDto dto)
        {
            var structuralResult = await _loginValidator.ValidateAsync(dto);
            if (!structuralResult.IsValid)
            {
                return BadRequest(new { errors = structuralResult.Errors.Select(e => new { property = e.PropertyName, message = e.ErrorMessage }) });
            }

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                // Mensaje genérico: no revela si el email o la contraseña fue el problema (FR-002).
                return Unauthorized(new { message = "Credenciales inválidas." });
            }

            var result = await _signInManager.PasswordSignInAsync(user, dto.Password, isPersistent: false, lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                return StatusCode(StatusCodes.Status423Locked, new { message = $"Cuenta bloqueada. Intenta nuevamente después de {lockoutEnd}." });
            }

            if (!result.Succeeded)
            {
                return Unauthorized(new { message = "Credenciales inválidas." });
            }

            var sessionDto = await BuildUserSessionDtoAsync(user);
            return Ok(sessionDto);
        }

        /// <summary>
        /// Cierra la sesión actual, invalidándola del lado del servidor.
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // SignOutAsync por sí solo solo limpia la cookie del lado del cliente — con cookies de
            // Identity sin estado, el token sigue siendo válido hasta su expiración si se reenvía
            // (ej. replay). Para satisfacer FR-004 ("la misma cookie no vuelve a dar acceso") de
            // verdad, se regenera el SecurityStamp del usuario: combinado con
            // SecurityStampValidatorOptions.ValidationInterval = TimeSpan.Zero (research.md Decisión
            // 15), la cookie usada para este logout queda inválida desde el siguiente request.
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                await _userManager.UpdateSecurityStampAsync(user);
            }

            await _signInManager.SignOutAsync();
            return NoContent();
        }

        /// <summary>
        /// Devuelve la información de la sesión actualmente autenticada.
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserSessionDto>> Me()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            return Ok(await BuildUserSessionDtoAsync(user));
        }

        private async Task<UserSessionDto> BuildUserSessionDtoAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? string.Empty;

            string? fullName = null;
            int? branchId = null;

            if (user.EmployeeId.HasValue)
            {
                var employee = await _unitOfWork.EmployeeRepository.SingleOrDefaultAsync(x => x.Id == user.EmployeeId.Value);
                if (employee != null)
                {
                    fullName = $"{employee.FirstName} {employee.LastName}";
                    branchId = employee.BranchId;
                }
            }

            return new UserSessionDto
            {
                UserId = user.Id,
                Email = user.Email ?? string.Empty,
                EmployeeId = user.EmployeeId,
                FullName = fullName,
                Role = role,
                BranchId = branchId
            };
        }
    }
}
