using Dsw2025Tpi.Data;
using Dsw2025Tpi.Domain.Interfaces;
using Dsw2025Tpi.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Dsw2025Tpi.Data.helpers;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Application.Services;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Dsw2025Tpi.Application.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TPI - DSW2025",
        Version = "v1",
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa 'Bearer ' y luego tu token en el cuadro de texto de abajo. Ejemplo: 'Bearer MITOKENJWT'"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
           Array.Empty<string>()
        }
    });

    options.EnableAnnotations();
});
builder.Services.AddHealthChecks();

var jwtConfig = builder.Configuration.GetSection("JwtConfig");
var keyText = jwtConfig["Key"] ?? throw new ArgumentNullException("JwtConfig:Key no está configurada.");
var key = Encoding.UTF8.GetBytes(keyText);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig["Issuer"],
            ValidAudience = jwtConfig["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddDbContext<Dsw2025TpiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IRepository, EfRepository>();
builder.Services.AddScoped<IProductsManagementService, ProductsManagementService>();
builder.Services.AddScoped<IOrderManagementService, OrdersManagementService>();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<Dsw2025TpiContext>();
        context.Database.Migrate(); context.Seedwork<Customer>("sources/Customers.json");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandler>();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/healthcheck");

app.Run();