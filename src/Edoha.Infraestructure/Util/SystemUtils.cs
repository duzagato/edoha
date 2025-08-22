using Edoha.Domain.Interfaces.Infraestructure.Context;
using Edoha.Domain.Interfaces.Infraestructure.Util;
using Edoha.Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Edoha.Infraestructure.Util
{
    public class SystemUtils : ISystemUtils
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly IRequestValidationContext _requestValidationContext;

        public SystemUtils(IHttpContextAccessor httpContext,
        IRequestValidationContext requestValidationContext)
        {
            _httpContext = httpContext;
            _requestValidationContext = requestValidationContext;
        }
        public string GetClientIp()
        {
            var context = _httpContext.HttpContext;

            if (context == null)
            {
                _requestValidationContext.AddError("LoginError", "Erro ao obter o IP");
                throw new RequestValidationException(_requestValidationContext.GetErrors());
            }
            else
            {
                var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                
                if (string.IsNullOrEmpty(ip))
                    ip = context.Connection.RemoteIpAddress?.ToString();

                return ip!;
            }
        }
    }
}
