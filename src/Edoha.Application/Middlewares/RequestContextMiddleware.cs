namespace Edoha.Application.Middlewares
{
    public class RequestContextMiddleware
    {
        private readonly RequestDelegate _next;
        public RequestContextMiddleware(RequestDelegate next) 
        {
            _next = next;
        }
    }
}
