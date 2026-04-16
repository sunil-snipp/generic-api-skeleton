using Generic.Api.Application.Reports.Abstractions;
using Generic.Api.Application.Reports.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Generic.Api.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAccessPolicyService, AccessPolicyService>();
        services.AddScoped<IReportService, ReportService>();

        return services;
    }
}
