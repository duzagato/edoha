using Edoha.Domain.Interfaces.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Net.Http;
using System.Security.Claims;

namespace Edoha.Infrastructure.Handlers
{
    public class PermissionRequirement : IAuthorizationRequirement { }

    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IUserPermissionService _userPermissionsService;
        private IHttpContextAccessor _httpContext { get; set; }

        public PermissionHandler(
            IUserPermissionService userPermissionsService,
            IHttpContextAccessor httpContext
        )
        {
            _userPermissionsService = userPermissionsService;
            _httpContext = httpContext;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            PermissionRequirement requirement
        )
        {
            var httpContext = _httpContext.HttpContext;

            var page = GetRequestPage(context, httpContext);
            var action = GetRequestAction(context, httpContext);
            var idUser = GetIdUser(context);

            var permission = await _userPermissionsService.GetUserActionByPageName(idUser, action, page);
        }

        private string GetRequestPage(AuthorizationHandlerContext context, HttpContext? httpContext)
        {
            var routeData = httpContext?.GetRouteData();
            var page = routeData?.Values["controller"]?.ToString();

            if (string.IsNullOrEmpty(page))
            {
                context.Fail();
                throw new Exception("Não foi encontrada uma página");
            }

            return page;
        }

        private string GetRequestAction(AuthorizationHandlerContext context, HttpContext? httpContext)
        {
            var action = httpContext?.Request.Method.ToString().ToLower();

            if (string.IsNullOrEmpty(action))
            {
                context.Fail();
                throw new Exception("Não foi encontrada uma ação");
            }

            return action;
        }

        private Guid GetIdUser(AuthorizationHandlerContext context)
        {
            var idUserClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(idUserClaim) || !Guid.TryParse(idUserClaim, out var idUser))
            {
                context.Fail();
                throw new Exception("Não foi encontrada um ID pro usuário");
            }

            return idUser;
        }
    }
}