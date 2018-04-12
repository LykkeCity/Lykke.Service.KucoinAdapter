using System.Linq;
using Common.Log;
using Lykke.Service.KucoinAdapter.Services.RestApi;
using Lykke.Service.KucoinAdapter.Services.RestApi.Models;
using Lykke.Service.KucoinAdapter.Settings;
using Lykke.SettingsReader;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.KucoinAdapter.Middleware
{
    public static class ClientTokenMiddleware
    {
        private const string CredsKey = "api-credentials";

        public static RestApiClient RestApi(this Controller controller)
        {
            if (controller.HttpContext.Items.TryGetValue(CredsKey, out var creds))
            {
                return (RestApiClient) creds;
            }

            return null;
        }

        public const string ClientTokenHeader = "X-API-KEY";

        public static void UseAuthenticationMiddleware(
            this IApplicationBuilder app,
            IReloadingManager<AppSettings> appSettings,
            ILog log)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Headers.TryGetValue(ClientTokenHeader, out var token))
                {
                    if (appSettings.CurrentValue.KucoinAdapterService.Clients.TryGetValue(
                        token.FirstOrDefault(),
                        out var creds))

                        if (creds != null)
                        {
                            context.Items[CredsKey] = new RestApiClient(creds, log);
                        }
                }
                else
                {
                    context.Items[CredsKey] = new RestApiClient(new ApiCredentials(), log);
                }

                await next();
            });
        }
    }
}
