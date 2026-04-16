using Microsoft.OpenApi;

namespace Generic.Api.Web.OpenApi;

public static class SwaggerExtensions
{
    public static IServiceCollection AddGenericApiSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        var title = configuration["OpenApi:Title"] ?? "Generic API";
        var version = configuration["OpenApi:Version"] ?? "v1";

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(version, new OpenApiInfo { Title = title, Version = version });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Paste JWT token only. Swagger adds Bearer prefix automatically.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

        });

        return services;
    }
    public static WebApplication UseGenericApiSwagger(this WebApplication app)
    {
        var version = app.Configuration["OpenApi:Version"] ?? "v1";
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{version}");
        });
        return app;
    }
}
