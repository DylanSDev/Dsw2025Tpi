using Dsw2025Tpi.Data;
using Microsoft.EntityFrameworkCore;
using Dsw2025Tpi.Data.helpers;
using Dsw2025Tpi.Domain.Entities;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Dsw2025Tpi.Api.Configurations;
using Dsw2025Tpi.Api.Db_Initialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen
(
    options =>
    {
        options.SwaggerDoc
        (
            "v1",
            new OpenApiInfo
            {
                Title = "TPI - DSW2025",
                Version = "v1",
            }
        );

        options.AddSecurityDefinition
        (
            "Bearer",
            new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Ingresa 'Bearer ' y luego tu token en el cuadro de texto de abajo. Ejemplo: 'Bearer MITOKENJWT'"
            }
        );

        options.AddSecurityRequirement
        (
            new OpenApiSecurityRequirement
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
            }
        );

        options.EnableAnnotations();
    }
);
builder.Services.AddHealthChecks();

builder.Services.AddIdentity<IdentityUser, IdentityRole>
(
    options => { options.Password.RequiredLength = 8; }
)
.AddEntityFrameworkStores<AuthenticateContext>()
.AddDefaultTokenProviders();

var jwtConfig = builder.Configuration.GetSection("JwtConfig");
var keyText = jwtConfig["Key"] ?? throw new ArgumentNullException("JwtConfig:Key no está configurada.");
var key = Encoding.UTF8.GetBytes(keyText);

builder.Services.AddAuthentication
(
    options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }
)
.AddJwtBearer
(
    options =>
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
    }
);

builder.Services.AddDomainServices(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFront", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()  
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();
DbInitializer.DbStart(app);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFront");

app.UseMiddleware<ExceptionHandler>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

await DbInitializer.DbIdentityStart(app);

app.MapHealthChecks("/healthcheck");

app.Run();