using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using ShipmentTracker.Core.Constants;
using ShipmentTracker.Core.DTOs.Auth;
using ShipmentTracker.Core.DTOs.Branches;
using ShipmentTracker.Core.DTOs.Customers;
using ShipmentTracker.Core.DTOs.Employees;
using ShipmentTracker.Core.DTOs.Orders;
using ShipmentTracker.Core.DTOs.ShipmentEvents;
using ShipmentTracker.Core.DTOs.Vehicles;
using ShipmentTracker.Core.Interfaces;
using ShipmentTracker.Core.Interfaces.Repositories;
using ShipmentTracker.Core.Interfaces.Services;
using ShipmentTracker.Infrastructure.Data;
using ShipmentTracker.Infrastructure.Data.Seed;
using ShipmentTracker.Core.Identity;
using ShipmentTracker.Infrastructure.Identity;
using ShipmentTracker.Infrastructure.Repositories;
using ShipmentTracker.Services;
using ShipmentTracker.Services.Validators.Auth;
using ShipmentTracker.Services.Validators.Branches;
using ShipmentTracker.Services.Validators.Customers;
using ShipmentTracker.Services.Validators.Employees;
using ShipmentTracker.Services.Validators.Orders;
using ShipmentTracker.Services.Validators.ShipmentEvents;
using ShipmentTracker.Services.Validators.Shipments;
using ShipmentTracker.Services.Validators.Vehicles;
using ShipmentTracker.Web.Authorization;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .WithExposedHeaders("X-Total-Count", "X-Page", "X-Page-Size", "X-Total-Pages");
    });
});

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(cfg => { }, typeof(ShipmentService).Assembly);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    x => x.MigrationsAssembly("ShipmentTracker.Infrastructure")));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>()
    .AddDefaultTokenProviders();

// Fuerza la revalidación (Employee activo + rol vigente) en cada request en vez de cada 30 min
// (research.md Decisión 15, FR-005/SC-003).
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.Zero;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    // None (no Strict/Lax) porque el frontend (http://localhost:3000) y esta API
    // (https://localhost:7156) difieren en esquema: los navegadores los tratan como cross-site
    // ("schemeful same-site"), y Strict/Lax nunca envían la cookie en fetch cross-site aunque
    // CORS permita credenciales. Secure ya está forzado arriba, requisito de SameSite=None.
    options.Cookie.SameSite = SameSiteMode.None;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
    // API pura: 401/403 en JSON en vez del redirect HTML por defecto de Identity.
    options.Events.OnRedirectToLogin = ctx =>
    {
        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = ctx =>
    {
        ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
    options.Events.OnValidatePrincipal = EmployeeSessionValidator.ValidateAsync;
});

builder.Services.AddAuthorization(options =>
{
    // Deniega por defecto cualquier endpoint sin [Authorize]/[AllowAnonymous] explícito (FR-010/FR-017):
    // defensa en profundidad frente a un futuro controlador que olvide decorar una acción nueva.
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
// SuperAdmin pasa cualquier [Authorize(Roles = "...")] sin listarlo explícitamente (research.md Decisión 6).
builder.Services.AddSingleton<IAuthorizationHandler, SuperAdminAuthorizationHandler>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IShipmentRepository, ShipmentRepository>();
builder.Services.AddScoped<IShipmentService, ShipmentService>();
builder.Services.AddScoped<IValidator<StatusTransitionContext>, ShipmentTransitionValidator>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IValidator<CreateBranchDto>, CreateBranchDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateBranchDto>, UpdateBranchDtoValidator>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IValidator<CreateEmployeeDto>, CreateEmployeeDtoValidator>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IValidator<CreateVehicleDto>, CreateVehicleDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateEmployeeDto>, UpdateEmployeeDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateVehicleDto>, UpdateVehicleDtoValidator>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IValidator<CreateIndividualCustomerDto>, CreateIndividualCustomerDtoValidator>();
builder.Services.AddScoped<IValidator<CreateBusinessCustomerDto>, CreateBusinessCustomerDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateCustomerDto>, UpdateCustomerDtoValidator>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IShipmentEventRepository, ShipmentEventRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IValidator<CreateOrderDto>, CreateOrderDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateOrderDto>, UpdateOrderDtoValidator>();
builder.Services.AddScoped<IDeliveryAttemptRepository, DeliveryAttemptRepository>();
builder.Services.AddScoped<IShipmentEventService, ShipmentEventService>();
builder.Services.AddScoped<IValidator<RegisterEventDto>, RegisterEventDtoValidator>();
builder.Services.AddScoped<IValidator<RegisterDeliveryAttemptDto>, RegisterDeliveryAttemptDtoValidator>();
builder.Services.AddScoped<IValidator<LoginDto>, LoginDtoValidator>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IValidator<CreateUserDto>, CreateUserDtoValidator>();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ShipmentTracker API",
        Version = "v1",
        Description = "API REST para la gesti�n y seguimiento de env�os.",
    });
    // Archivo XML del proyecto Web
    var webXmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, webXmlFilename));

    // Archivo XML del proyecto Core (donde est�n los DTOs)
    var coreXmlFilename = "ShipmentTracker.Core.xml";
    var coreXmlPath = Path.Combine(AppContext.BaseDirectory, coreXmlFilename);
    if (File.Exists(coreXmlPath))
    {
        options.IncludeXmlComments(coreXmlPath);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await IdentitySeeder.SeedAsync(app.Services);

app.Run();

