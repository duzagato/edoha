using System.Text.Json;
using Edoha.Shared.Exceptions;

namespace Edoha.Application.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        
        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (RequestValidationException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";

                var result = new
                {
                    is_valid = false,
                    errors = ex.Errors
                };

                var json = JsonSerializer.Serialize(result);
                await context.Response.WriteAsync(json);
            }
            catch (Exception ex)
            {
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
