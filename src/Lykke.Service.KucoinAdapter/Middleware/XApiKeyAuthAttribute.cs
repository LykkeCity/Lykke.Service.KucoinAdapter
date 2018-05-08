using System.Collections.Generic;
using System.Linq;
using Lykke.Service.KucoinAdapter.Services.RestApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Lykke.Service.KucoinAdapter.Middleware
{
    public sealed class XApiKeyAuthAttribute : ActionFilterAttribute
    {
        internal static IReadOnlyDictionary<string, ApiCredentials> Credentials { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(ClientTokenMiddleware.ClientTokenHeader, out var h)
                || h.Count != 1)
            {
                context.Result = new BadRequestObjectResult(
                    $"Header {ClientTokenMiddleware.ClientTokenHeader} with single value is required");
                return;
            }

            if (!Credentials.TryGetValue(h[0], out var creds))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
