using Amazon.Lambda.AspNetCoreServer.Hosting;
using Edoha.Application;
using Edoha.Application.Middlewares;
using Serilog;
using Serilog.Enrichers.CorrelationId;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

var loggerConfig = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithCorrelationId()
    .MinimumLevel.Information();

if (builder.Environment.IsProduction())
    loggerConfig.WriteTo.Console(new RenderedCompactJsonFormatter());
else
    loggerConfig.WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception} {CorrelationId}");

Log.Logger = loggerConfig.CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddJwt(builder.Configuration);
builder.Services.AddFactories();
builder.Services.AddRepositories();
builder.Services.AddUtils();
builder.Services.AddDomainServices();

builder.Services.AddHealthChecks();

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        if (allowedOrigins.Length > 0)
            policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

Log.Information("API Edoha iniciada em ambiente: {Ambiente}", app.Environment.EnvironmentName);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
