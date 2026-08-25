using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Application.Services.Interfaces;
using Dsw2025Tpi.Data;
using Dsw2025Tpi.Data.Repositories;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dsw2025Tpi.Api.Configurations;

public static class DomainServicesConfigurationExtension
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<Dsw2025TpiContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddDbContext<AuthenticateContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped<IRepository, EfRepository>();
        services.AddScoped<IProductsManagementService, ProductsManagementService>();
        services.AddScoped<IOrderManagementService, OrdersManagementService>();
        services.AddScoped<IDashboardManagementService, DashboardManagementService>();
        services.AddSingleton<JwtTokenService>();

        return services;
    }
}