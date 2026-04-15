using Generic.Api.Application.Abstractions.ExternalIdentity;
using Generic.Api.Application.Abstractions.PowerBi;
using Generic.Api.Application.Abstractions.Reports;
using Generic.Api.Application.Services;
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
