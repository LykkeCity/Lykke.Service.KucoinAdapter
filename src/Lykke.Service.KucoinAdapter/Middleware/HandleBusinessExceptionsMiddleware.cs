using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.KucoinAdapter.Services.RestApi.Models.Fails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Lykke.Service.KucoinAdapter.Middleware
{
    public static class HandleBusinessExceptionsMiddleware
    {
        public static void UseHandleBusinessExceptionsMiddleware(this IApplicationBuilder app)
        {
            app.Use(SetStatusOnBusinessError);
        }

        private static async Task SetStatusOnBusinessError(HttpContext httpContext, Func<Task> next)
        {
            try
            {
                await next();
            }
            catch (VolumeTooSmallException)
            {
                MakeBadRequest(httpContext, "volumeTooSmall");
            }
            catch (InvalidOrderIdException)
            {
                MakeBadRequest(httpContext, "orderIdFormat");
            }
            catch (NoBalanceException)
            {
                MakeBadRequest(httpContext, "notEnoughBalance");
            }
            catch (InvalidInstrumentException)
            {
                MakeBadRequest(httpContext, "instrumentIsNotSupported");
            }
        }

        private static void MakeBadRequest(HttpContext httpContext, string error)
        {
            using (var body = new MemoryStream(Encoding.UTF8.GetBytes(error)))
            {
                httpContext.Response.ContentType = "text/plain";
                httpContext.Response.StatusCode = 400;
                body.CopyTo(httpContext.Response.Body);
            }
        }
    }
}
