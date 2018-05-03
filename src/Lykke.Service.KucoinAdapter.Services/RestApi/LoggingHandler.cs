using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;

namespace Lykke.Service.KucoinAdapter.Services.RestApi
{
    public class LoggingHandler : DelegatingHandler
    {
        private readonly ILog _log;

        public LoggingHandler(ILog log, HttpMessageHandler next) : base(next)
        {
            _log = log;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var query = $"{request.Method} {request.RequestUri}";
            if (request.Method == HttpMethod.Post)
            {
                query += " <- " + await request.Content.ReadAsStringAsync();
            }

            var sw = Stopwatch.StartNew();
            try
            {
                var result = await base.SendAsync(request, cancellationToken);

                if (!IgnoreSuccess(request.RequestUri))
                {
                    _log.WriteInfo(nameof(LoggingHandler), query, $"send has taken {sw.Elapsed}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _log.WriteInfo(nameof(LoggingHandler), query, $"Request to {query} failed in {sw.Elapsed}: " + ex);
                throw;
            }
        }

        private static readonly HashSet<string> PathsToIngore = new HashSet<string>
        {
            "/v1/open/orders"
        };

        private static bool IgnoreSuccess(Uri requestRequestUri)
        {
            if (PathsToIngore.Contains(requestRequestUri.AbsolutePath))
            {
                return true;
            }

            return false;
        }
    }
}
