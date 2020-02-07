using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;

namespace DualJwt.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AuthorizeJwtAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public AuthorizeJwtAttribute()
        {
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Unauthorized);
                return;
            }

            var userId = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
                return;
            }

            SetCurrentPrincipal(context);
        }

        private void SetCurrentPrincipal(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated && context.HttpContext.User.Identity.AuthenticationType == "JWT")
            {

                var identity = (ClaimsIdentity)context.HttpContext.User.Identity;
                var panelIdentity = new PanelUserIdentity()
                {
                    IsAuthenticated = identity.IsAuthenticated,
                    UserId = identity.Claims.FirstOrDefault(x => x.Type == "UserId").Value
                };

                Thread.CurrentPrincipal = new PanelUserPrincipal(panelIdentity);
                return;
            }

            var principal = new PanelUserPrincipal(
                new PanelUserIdentity()
                {
                    IsAuthenticated = true,
                    UserId = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value,
                });
            Thread.CurrentPrincipal = principal;
        }
    }
}
