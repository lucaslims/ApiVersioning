using ApiVersioning.Data;
using ApiVersioning.Interfaces;
using ApiVersioning.Services;
using ApiVersioning.Swagger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ApiVersioning.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        //DataContext
        services.AddDbContext<DataContext>(options =>
        {
            options.EnableDetailedErrors();
            options.UseSqlServer(config.GetConnectionString("DefaultConnection"));
        });

        //Swagger
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        // //Repositories
        // services.AddTransient<IUsersRepository, UsersRepository>();

        // //Services
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}
