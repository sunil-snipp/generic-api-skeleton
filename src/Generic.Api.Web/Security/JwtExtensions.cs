using System.Text;
using Microsoft.AspNetCore.Authentication;
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

        if (!string.IsNullOrWhiteSpace(signingKey) && signingKey.Length >= 32)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var issuer = jwtSection["Issuer"] ?? "Generic.Api";
                    var audience = jwtSection["Audience"] ?? "Generic.Api";
                    var keyBytes = Encoding.UTF8.GetBytes(signingKey);

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
        }
        else
        {
            services
                .AddAuthentication("PassThroughBearer")
                .AddScheme<AuthenticationSchemeOptions, PassThroughAuthenticationHandler>("PassThroughBearer", _ => { });
        }

        services.AddAuthorization();
        return services;
    }
}
