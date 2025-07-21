using Edoha.Application;
using Edoha.Application.Middlewares;
using Serilog;
using Serilog.AspNetCore;
using Serilog.Enrichers.CorrelationId;

var builder = WebApplication.CreateBuilder(args);

// Configurando Serilog como logger principal
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithCorrelationId()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception} {CorrelationId}")
    .MinimumLevel.Information() // ?? Corrigido: não existe `.LoggerMinimumLevelConfiguration` nem `.information()`
    .CreateLogger();

// Substitui o logger padrão do ASP.NET Core por Serilog
builder.Host.UseSerilog();

// Serviços
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddUtils();
builder.Services.AddDomainServices();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

Log.Information("API Edoha iniciada em ambiente: {Ambiente}", app.Environment.EnvironmentName);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middlewares
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
