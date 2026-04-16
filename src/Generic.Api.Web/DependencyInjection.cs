using Generic.Api.Application.Common;
using Generic.Api.Web.OpenApi;
using Generic.Api.Web.Security;
using Generic.Api.Web.Services;
using Microsoft.AspNetCore.RateLimiting;

namespace Generic.Api.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IRequestContext, HttpRequestContext>();
        services.AddScoped<ICurrentAccessTokenProvider, HttpContextAccessTokenProvider>();

        // NOTE: IStructuredLogger is registered in Infrastructure.AddInfrastructure()
        // No need to register it here

        services.AddExceptionHandler<Errors.GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddGenericApiJwtAuthentication(configuration);

        services.AddControllers();
        services.AddGenericApiSwagger(configuration);

        services.AddHealthChecks();

        services.AddCors(options =>
        {
            options.AddPolicy("Default", policy =>
            {
                var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
                if (origins is { Length: > 0 })
                {
                    policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
                }
                else
                {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                }
            });
        });

        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("default", o =>
            {
                o.PermitLimit = configuration.GetValue("RateLimiting:PermitLimit", 200);
                o.Window = TimeSpan.FromMinutes(1);
                o.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                o.QueueLimit = configuration.GetValue("RateLimiting:QueueLimit", 0);
            });
        });

        return services;
    }
}
