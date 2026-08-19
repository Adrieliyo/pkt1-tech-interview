using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipmentTracker.Core.Constants;
using ShipmentTracker.Core.DTOs.Auth;
using ShipmentTracker.Core.Interfaces.Services;

namespace ShipmentTracker.Web.Controllers
{
    /// <summary>
    /// Aprovisionamiento de cuentas de staff. Exclusivo de SuperAdmin.
    /// </summary>
    [Route("api/users")]
    [ApiController]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Aprovisiona una nueva cuenta de acceso para un Employee existente y activo que aún no
        /// tiene una, asignándole el rol de Identity correspondiente a su EmployeeRole.
        /// </summary>
        /// <param name="dto">Employee a vincular y contraseña inicial.</param>
        /// <returns>La sesión recién creada (sin iniciar sesión automáticamente).</returns>
        [HttpPost]
        [Authorize(Roles = Roles.SuperAdmin)]
        public async Task<ActionResult<UserSessionDto>> CreateUser([FromBody] CreateUserDto dto)
        {
            try
            {
                var result = await _userService.CreateUserForEmployeeAsync(dto);
                return Created($"/api/users/{result.UserId}", result);
            }
            catch (FluentValidation.ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors.Select(e => new { property = e.PropertyName, message = e.ErrorMessage }) });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
