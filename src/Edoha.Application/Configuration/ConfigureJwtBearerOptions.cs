using Edoha.Domain.Interfaces.Infraestructure.Services;
using Edoha.Infraestructure.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection; // IMPORTANTE
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
{
    // Mudamos aqui: Injetamos o ServiceProvider que é Singleton
    private readonly IServiceProvider _serviceProvider;

    public ConfigureJwtBearerOptions(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name != JwtBearerDefaults.AuthenticationScheme) return;
        Configure(options);
    }

    public void Configure(JwtBearerOptions options)
    {
        string secretKey;

        // Criamos um escopo manual para usar o serviço Scoped de forma segura
        using (var scope = _serviceProvider.CreateScope())
        {
            var secretsManager = scope.ServiceProvider.GetRequiredService<ISecretsManagerService>();
            secretKey = secretsManager.GetJwtKeyAsync().Result;
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = JwtSettings.Issuer,
            ValidAudience = JwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };

        // Seus eventos continuam aqui...
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context => { /* ... seu codigo ... */ return Task.CompletedTask; },
            OnChallenge = context => { /* ... seu codigo ... */ return Task.CompletedTask; },
            OnForbidden = context => { /* ... seu codigo ... */ return Task.CompletedTask; }
        };
    }
}