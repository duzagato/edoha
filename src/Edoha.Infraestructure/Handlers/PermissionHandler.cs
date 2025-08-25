using Edoha.Domain.Entities;
using Action = Edoha.Domain.Entities.Action;
using Edoha.Domain.Interfaces.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
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
            try
            {
                var httpContext = _httpContext.HttpContext;

                var page = GetRequestPage(context, httpContext);
                var action = GetRequestAction(context, httpContext);
                var idUser = GetIdUser(context);
                var permission = await GetPermissionForThisPage(idUser, page, action);

                if (permission != null)
                {
                    context.Succeed(requirement);
                }
            }
            catch
            {
                context.Fail();
            }
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

        private async Task<Action> GetPermissionForThisPage(Guid idUser, string page, string action)
        {
            var permission = await _userPermissionsService.GetUserActionByPageName(idUser, page, action);

            if(permission is null)
            {
                throw new Exception("Não possui permissão");
            }
            else
            {
                return permission;
            }
        }
    }
}