using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using ShipmentTracker.Core.DTOs;
using ShipmentTracker.Core.Interfaces;
using ShipmentTracker.Core.Interfaces.Repositories;
using ShipmentTracker.Core.Interfaces.Services;
using ShipmentTracker.Infrastructure.Data;
using ShipmentTracker.Infrastructure.Repositories;
using ShipmentTracker.Services;
using ShipmentTracker.Services.Validators;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .WithExposedHeaders("X-Total-Count", "X-Page", "X-Page-Size", "X-Total-Pages");
    });
});

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(cfg => { }, typeof(Program).Assembly, typeof(ShipmentService).Assembly);

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), 
    x => x.MigrationsAssembly("ShipmentTracker.Infrastructure")));

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
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IValidator<CreateEmployeeDto>, CreateEmployeeDtoValidator>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IValidator<CreateVehicleDto>, CreateVehicleDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateEmployeeDto>, UpdateEmployeeDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateVehicleDto>, UpdateVehicleDtoValidator>();

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

app.MapControllers();

app.Run();

