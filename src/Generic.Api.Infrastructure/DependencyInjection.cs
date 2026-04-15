using Generic.Api.Application.Abstractions.ExternalIdentity;
using Generic.Api.Application.Abstractions.PowerBi;
using Generic.Api.Application.Configuration;
using Generic.Api.Infrastructure.Persistence;
using Generic.Api.Infrastructure.Integrations.ExternalIdentity;
using Generic.Api.Infrastructure.Integrations.PowerBi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Generic.Api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")
            ?? "Data Source=generic-api.db";

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));

        services
            .AddOptions<PowerBiOptions>()
            .Bind(configuration.GetSection(PowerBiOptions.SectionName));

        services
            .AddOptions<ExternalIdentityOptions>()
            .Bind(configuration.GetSection(ExternalIdentityOptions.SectionName));

        services.AddHttpClient<IExternalIdentityClient, ExternalIdentityClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<ExternalIdentityOptions>>().Value;
            if (!string.IsNullOrWhiteSpace(options.BaseUrl))
            {
                client.BaseAddress = new Uri(options.BaseUrl);
            }
        });

        services.AddScoped<IPowerBiClient, PowerBiClient>();

        return services;
    }
}
