using Generic.Api.Application.Auth.Abstractions;
using Generic.Api.Application.Logging.Abstractions;
using Generic.Api.Application.Reports;
using Generic.Api.Application.Reports.Abstractions;
using Generic.Api.Infrastructure.ExternalIdentity;
using Generic.Api.Infrastructure.Logging;
using Generic.Api.Infrastructure.PowerBi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Generic.Api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<ReportOptions>()
            .Bind(configuration.GetSection(ReportOptions.SectionName));

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

        // Register logging implementation
        services.AddScoped<IStructuredLogger>(sp => 
            new SerilogStructuredLogger(sp.GetRequiredService<Serilog.ILogger>()));

        return services;
    }
}
