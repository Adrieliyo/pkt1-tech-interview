using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShipmentTracker.Core.Interfaces;
using ShipmentTracker.Core.Interfaces.Repositories;
using ShipmentTracker.Core.Interfaces.Services;
using ShipmentTracker.Infrastructure.Data;
using ShipmentTracker.Infrastructure.Repositories;
using ShipmentTracker.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(cfg => { }, typeof(Program).Assembly);

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), 
    x => x.MigrationsAssembly("ShipmentTracker.Infrastructure")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IShipmentRepository, ShipmentRepository>();
builder.Services.AddScoped<IShipmentService, ShipmentService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

