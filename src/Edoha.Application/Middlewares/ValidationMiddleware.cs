using System.Text.Json;
using Edoha.Shared.Exceptions;

namespace Edoha.Application.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("ExceptionHandlerMiddleware iniciado!");
            try
            {
                await _next(context);
                _logger.LogInformation("Middleware encerrado sem erros!");
            }
            catch (RequestValidationException ex)
            {
                _logger.LogInformation("Erro de validação. Um ou mais campos inválidos!");
                _logger.LogInformation("Resultado da validação: {Errors}", ex.Errors);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";

                var result = new
                {
                    is_valid = false,
                    errors = ex.Errors
                };

                var json = JsonSerializer.Serialize(result);
                await context.Response.WriteAsync(json);
                _logger.LogInformation("ExceptionHandlerMiddleware finalizado!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro não esperado.");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    message = "Ocorreu um erro interno no servidor."
                }));
            }
        }
    }

}
