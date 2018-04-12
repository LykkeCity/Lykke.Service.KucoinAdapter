using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Lykke.Service.KucoinAdapter.Services.RestApi.Models;
using MoreLinq;

namespace Lykke.Service.KucoinAdapter.Services.RestApi
{

    public sealed class KucoinAuthenticationHandler : DelegatingHandler
    {
        private readonly ApiCredentials _credentials;

        public KucoinAuthenticationHandler(ApiCredentials credentials, HttpMessageHandler next)
            : base(next)
        {
            _credentials = credentials;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var parameters = request.RequestUri.ParseQueryString();

            if (request.Content is FormUrlEncodedContent form)
            {
                var formParameters = await form.ReadAsFormDataAsync(ct);

                foreach (var k in formParameters.AllKeys)
                {
                    parameters.Add(k, formParameters[k]);
                }
            }

            var queryString = string.Join(
                "&",
                parameters.AllKeys.OrderBy(
                        x => x,
                        StringComparer.InvariantCulture,
                        OrderByDirection.Ascending)
                    .Select(x => $"{x}={parameters[x]}"));

            var nonce = Nonce().ToString();

            var parts = new[]
            {
                request.RequestUri.AbsolutePath,
                nonce,
                queryString
            };

            var forSign = string.Join("/",  parts);

            var encodedForSign = Convert.ToBase64String(Encoding.UTF8.GetBytes(forSign));

            var signature = CreateSignature(encodedForSign);

            request.Headers.Add("KC-API-KEY", _credentials.ApiKey);
            request.Headers.Add("KC-API-NONCE", nonce);
            request.Headers.Add("KC-API-SIGNATURE", signature.ToLower());

            return await base.SendAsync(request, ct);
        }

        public string CreateSignature(string forSign)
        {
            using (var hmac = _credentials.CreateHmac())
            {
                return hmac.ComputeHash(Encoding.UTF8.GetBytes(forSign)).ToHexString();
            }
        }

        private static long Nonce()
        {
            return (long) DateTime.UtcNow.ToUnixTime();
        }
    }
}
