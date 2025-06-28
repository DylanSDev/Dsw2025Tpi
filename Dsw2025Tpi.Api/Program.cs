using Dsw2025Tpi.Data; // Asegúrate de agregar esta referencia
using Dsw2025Tpi.Domain.Interfaces; // Asegúrate de agregar esta referencia
using Dsw2025Tpi.Data.Repositories; // Asegúrate de agregar esta referencia
using Microsoft.EntityFrameworkCore; // Asegúrate de agregar esta referencia
using Dsw2025Tpi.Data.helpers; // Asegúrate de agregar esta referencia
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Application.Services; // Asegúrate de agregar esta referencia

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

// 1. Configurar DbContext con SQL Server
builder.Services.AddDbContext<Dsw2025TpiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Inyectar el repositorio
builder.Services.AddScoped<IRepository, EfRepository>();

// Inyectar el servicio de gestión de productos y ordenes
builder.Services.AddScoped<IProductsManagementService, ProductsManagementService>();
builder.Services.AddScoped<IOrderManagementService, OrdersManagementService>();

var app = builder.Build();

// 3. Aplicar migraciones y seedear los datos
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<Dsw2025TpiContext>();
        context.Database.Migrate(); // Aplica las migraciones pendientes
        context.Seedwork<Customer>("sources/Customers.json"); // Usa el método de extensión para seedear los clientes
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/healthcheck");

app.Run();