using Generic.Api.Application;
using Generic.Api.Infrastructure;
using Generic.Api.Infrastructure.Persistence;
using Generic.Api.Web;
using Generic.Api.Web.Middleware;
using Generic.Api.Web.OpenApi;
using Generic.Api.Web.Security;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddWebServices(builder.Configuration);

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    app.UseExceptionHandler();
    app.UseStatusCodePages();

    app.UseMiddleware<CorrelationIdMiddleware>();

    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });

    app.UseCors("Default");

    if (JwtExtensions.IsJwtConfigured(app.Configuration))
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }

    app.UseRateLimiter();

    if (app.Environment.IsDevelopment())
    {
        app.UseGenericApiSwagger();
    }

    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
        Predicate = _ => false
    });
    app.MapHealthChecks("/health/ready");

    app.MapControllers();

    if (app.Configuration.GetValue("Database:RunMigrationsOnStartup", app.Environment.IsDevelopment()))
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();
    }

    await app.RunAsync();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    await Log.CloseAndFlushAsync();
}
