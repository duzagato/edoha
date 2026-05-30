using Amazon.Lambda.AspNetCoreServer.Hosting;
using Edoha.Application;
using Edoha.Application.Middlewares;
using Serilog;
using Serilog.Enrichers.CorrelationId;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);

// O .NET já captura o ambiente nativamente aqui:
var environment = builder.Environment.EnvironmentName;

#region 1. Configuração de Logs (Serilog)
var loggerConfig = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithCorrelationId()
    .MinimumLevel.Information();

loggerConfig.WriteTo.Console(new RenderedCompactJsonFormatter());
Log.Logger = loggerConfig.CreateLogger();

builder.Services.AddMemoryCache();
builder.Host.UseSerilog();
#endregion

#region 2. Injeção de Dependência (Services)
builder.Services.AddHttpContextAccessor();

// Seus métodos de extensão da aplicação
builder.Services.AddFactories();
builder.Services.AddRepositories();
builder.Services.AddUtils();
builder.Services.AddDomainServices(builder.Configuration);

builder.Services.AddHealthChecks();

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

// Configurações do JWT que isolamos
builder.Services.ConfigureOptions<ConfigureAuthenticationOptions>();
builder.Services.ConfigureOptions<ConfigureJwtBearerOptions>();

builder.Services.AddAuthentication()
                .AddJwtBearer();

builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

#region 3. Configuração do CORS
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

if (allowedOrigins.Length == 0)
{
    Log.Warning("CORS Configuration Alert: No origins found in 'Cors:AllowedOrigins'. All cross-origin requests will be blocked.");
}
else
{
    Log.Information("CORS Configuration Loaded. Allowed origins: {@AllowedOrigins}", allowedOrigins);
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            if (allowedOrigins.Contains("*"))
            {
                policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            }
            else
            {
                policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
            }
        }
    });
});
#endregion

// --- FIM DA ETAPA DE CONFIGURAÇÃO / DI ---
var app = builder.Build();

#region 4. Pipeline de Middlewares (A ORDEM AQUI IMPORTA MUITO)

// O primeiro precisa ser o tratamento de erros global
app.UseMiddleware<ExceptionHandlingMiddleware>();

Log.Information("API Edoha iniciada em ambiente: {Ambiente}", environment);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // Removido o HttpsRedirection daqui de dentro
}

// Se for usar Redirection, o lugar correto é aqui (antes do Routing), 
// mas lembre-se: se a Lambda der erro de redirecionamento na AWS, comente a linha abaixo.
app.UseHttpsRedirection();

app.UseRouting();

// CORS sempre DEPOIS de UseRouting e ANTES de UseAuthentication
app.UseCors("AllowAngular");

// Autenticação sempre ANTES da Autorização
app.UseAuthentication();
app.UseAuthorization();

// Mapeamento dos endpoints (última etapa)
app.MapHealthChecks("/health");
app.MapControllers();
#endregion

app.Run();