using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace YourApp.Application.DependencyInjection
{
    public static class JwtInjection
    {
        public static IServiceCollection AddJwt(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                    };
                });

            return services;
        }
    }
}
