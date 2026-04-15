using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Generic.Api.Web.Security;

public static class JwtExtensions
{
    public static IServiceCollection AddGenericApiJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection("Jwt");
        var signingKey = jwtSection["SigningKey"];
        if (string.IsNullOrWhiteSpace(signingKey) || signingKey.Length < 32)
        {
            return services;
        }

        var issuer = jwtSection["Issuer"] ?? "Generic.Api";
        var audience = jwtSection["Audience"] ?? "Generic.Api";

        var keyBytes = Encoding.UTF8.GetBytes(signingKey);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        services.AddAuthorization();
        return services;
    }

    public static bool IsJwtConfigured(IConfiguration configuration)
    {
        var key = configuration["Jwt:SigningKey"];
        return !string.IsNullOrWhiteSpace(key) && key.Length >= 32;
    }
}
