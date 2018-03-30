using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Service.KucoinAdapter.Services.RestApi.Models;

namespace Lykke.Service.KucoinAdapter.Services.RestApi
{
    public static class KucoinHttpExtensions
    {
        public static async Task<T> ReadAsKucoin<T>(
            this HttpResponseMessage response,
            CancellationToken ct = default(CancellationToken))
        {
            response.EnsureSuccessStatusCode();
            var typed = await response.Content.ReadAsAsync<KucoinResponse<T>>(ct);
            typed.EnsureNoError();
            return typed.Data;
        }
    }
}
