using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Service.KucoinAdapter.Services.RestApi.Models;

namespace Lykke.Service.KucoinAdapter.Services.RestApi
{
    public static class KucoinHttpExtensions
    {
        public static DateTime FromKuckoinDateTime(this long milliseconds)
        {
            return new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds(milliseconds);
        }

        public static async Task<T> ReadAsKucoin<T>(
            this HttpResponseMessage response,
            CancellationToken ct = default(CancellationToken))
        {
            if (!response.IsSuccessStatusCode)
            {
                var msg = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Response status does not indicate success: " +
                                               $"{response.StatusCode:D} {response.StatusCode:G} ({msg})");
            }

            var typed = await response.Content.ReadAsAsync<KucoinResponse<T>>(ct);
            typed.EnsureNoError();
            return typed.Data;
        }
    }
}
