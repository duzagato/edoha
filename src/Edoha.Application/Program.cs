using Edoha.Application;
using Edoha.Application.Middlewares;
using Serilog;
using Serilog.AspNetCore;
using Serilog.Enrichers.CorrelationId;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

// Configurando Serilog como logger principal
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithCorrelationId()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception} {CorrelationId}")
    .MinimumLevel.Information() // ?? Corrigido: não existe `.LoggerMinimumLevelConfiguration` nem `.information()`
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddJwt(builder.Configuration);

// Serviços
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddUtils();
builder.Services.AddDomainServices();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

app.UseRouting();
app.UseCors("AllowAngular");

Log.Information("API Edoha iniciada em ambiente: {Ambiente}", app.Environment.EnvironmentName);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers();
app.UseCors("AllowAngular");

app.Run();
