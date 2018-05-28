using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.KucoinAdapter.Services.RestApi.Models;

namespace Lykke.Service.KucoinAdapter.Services.RestApi
{
    public sealed class RestApiClient
    {
        private readonly HttpClient _client;

        public RestApiClient(ApiCredentials credentials, ILog log)
        {
            var logger = new LoggingHandler(log, new HttpClientHandler());

            _client = new HttpClient(new KucoinAuthenticationHandler(credentials, logger))
            {
                BaseAddress = new Uri("https://api.kucoin.com/v1/")
            };
        }


        public async Task<ExchangeRateOfCoins> GetExchangeRateOfCoins(
            CancellationToken ct = default(CancellationToken))
        {
            var response = await _client.GetAsync("open/currencies", ct);
            return await response.ReadAsKucoin<ExchangeRateOfCoins>(ct: ct);
        }

        public async Task<IReadOnlyCollection<GetBalanceDataElement>> GetBalance(
            CancellationToken ct = default(CancellationToken))
        {
            const int limit = 12;
            var pageNumber = 1;
            var balances = new List<GetBalanceDataElement>();
            long total = 0;

            long pagesCount;
            do
            {
                var response = await _client.GetAsync($"account/balances?limit={limit}&page={pageNumber}", ct);
                var pageResponse = await response.ReadAsKucoin<GetBalanceResponse>(ct: ct);
                balances.AddRange(pageResponse.Datas);
                pageNumber += 1;
                pagesCount = pageResponse.TotalPages;
                total = pageResponse.Total;
            } while (pageNumber <= pagesCount);

            if (total != balances.Count)
            {
                throw new Exception($"Expected {total} balances, {balances.Count} received");
            }

            return balances;
        }

        public async Task<IReadOnlyCollection<GetSymbolsElement>> GetSymbols(
            CancellationToken ct = default(CancellationToken))
        {
            var response = await _client.GetAsync("market/symbols", ct);
            return await response.ReadAsKucoin<GetSymbolsElement[]>(ct: ct);
        }

        public async Task<KucoinOrderbook> GetOrderbook(
            string instrument,
            uint limit,
            CancellationToken ct = default(CancellationToken))
        {
            var response = await _client.GetAsync($"open/orders?symbol={WebUtility.UrlEncode(instrument)}&limit={limit}", ct);
            return await response.ReadAsKucoin<KucoinOrderbook>(ct);
        }
    }
}
