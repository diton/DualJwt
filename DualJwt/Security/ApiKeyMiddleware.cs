using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DualJwt.Security
{
    public static class ApiKeyMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiKeyAuthentication(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiKeyMiddleware>();
        }
    }

    public class ApiKeyMiddleware
    {
        private const string API_KEY_HEADER = "X-Api-Key";
        private readonly IApiKeyService _apiKeyService;

        public ApiKeyMiddleware(RequestDelegate next, IApiKeyService apiKeyService)
        {
            _next = next;
            _apiKeyService = apiKeyService;
        }
        private readonly RequestDelegate _next;

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments(new PathString("/api")))
            {
                if (context.Request.Headers.TryGetValue(API_KEY_HEADER, out var apiKeyHeaderValue))
                {
                    await TryAuthenticateApiKey(context, apiKeyHeaderValue);
                    await _next(context);
                }
                else
                {
                    await _next(context);
                }
            }
            else
            {
                await _next(context);
            }
        }

        private async Task TryAuthenticateApiKey(HttpContext context, string apiKeyHeaderValue)
        {
            var externalSystem = _apiKeyService.GetExternalServiceByApiKey(apiKeyHeaderValue);
            if (externalSystem == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Invalid API Key");
            }

            var controllerAction = GetControllerAction(context);

            if (!externalSystem.CanAccessControllerAction(controllerAction))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await context.Response.WriteAsync("You have no access to perform this action");
            }

            SetCurrentApiKeyPrincipal(context, externalSystem);
        }

        private AllowedControllerAction GetControllerAction(HttpContext context)
        {
            var path = context.Request.Path.ToUriComponent().Split('/');
            string controller = string.Empty;
            string action = string.Empty;

            // todo: need better way to determine request controller/action
            for (int i = 0; i < path.Length; i++)
            {
                if (path[i].Equals("api", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (i < path.Length - 1)
                    {
                        controller = path[i + 1];
                    }
                    if (i < path.Length - 2)
                    {
                        action = path[i + 2];
                    }
                    break;
                }
            }
            if (string.IsNullOrWhiteSpace(controller)) return null;

            return new AllowedControllerAction()
            {
                Action = action,
                Controller = controller,
                Method = context.Request.Method
            };
        }

        private void SetCurrentApiKeyPrincipal(HttpContext context, ExternalService service)
        {
            var identity = new PanelUserIdentity()
            {
                IsAuthenticated = true,
                UserId = service.Id.ToString()
            };

            context.User = new ClaimsPrincipal(identity);
            ((ClaimsIdentity)context.User.Identity).AddClaim(new Claim("UserId", service.Id.ToString()));
        }
    }

}
