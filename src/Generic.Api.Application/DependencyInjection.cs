using Generic.Api.Application.Reports;
using Generic.Api.Application.Reports.Ports;
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
