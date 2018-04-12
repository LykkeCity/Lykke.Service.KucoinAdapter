using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Service.KucoinAdapter.Core.Domain.SharedContracts;
using Lykke.Service.KucoinAdapter.Services.RestApi.Models;
using Newtonsoft.Json.Linq;

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
            return await response.ReadAsKucoin<ExchangeRateOfCoins>(ct);
        }

        public async Task<IReadOnlyCollection<GetBalanceDataElement>> GetBalance(
            CancellationToken ct = default(CancellationToken))
        {
            const int limit = 12;
            var pageNumber = 1;
            var balances = new List<GetBalanceDataElement>();
            long total;

            long pagesCount;
            do
            {
                var response = await _client.GetAsync($"account/balances?limit={limit}&page={pageNumber}", ct);
                var pageResponse = await response.ReadAsKucoin<GetBalanceResponse>(ct);
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
            return await response.ReadAsKucoin<GetSymbolsElement[]>(ct);
        }

        public async Task<KucoinOrderbook> GetOrderbook(
            KucoinInstrument instrument,
            uint limit,
            CancellationToken ct = default(CancellationToken))
        {
            using (var response = await _client.GetAsync(
                $"open/orders?symbol={WebUtility.UrlEncode(instrument.Value)}&limit={limit}",
                ct))
            {
                return await response.ReadAsKucoin<KucoinOrderbook>(ct);
            }
        }

        public async Task<ActiveOrders> GetActiveOrders(
            KucoinInstrument instrument,
            CancellationToken ct = default(CancellationToken))
        {
            using (var response = await _client.GetAsync(
                $"order/active?symbol={WebUtility.UrlEncode(instrument.Value)}",
                ct))
            {
                var kr = await response.ReadAsKucoin<ActiveOrdersListResponse>(ct);
                return new ActiveOrders(kr);
            }
        }

        public async Task<byte[]> CreateLimitOrder(
            KucoinInstrument kucoinInstrument,
            decimal amount,
            decimal price,
            TradeType tradeType,
            CancellationToken ct = default(CancellationToken))
        {
            var ktt = ConvertTradeType(tradeType);

            var command = new CreateLimitOrder
            {
                TradeType = ktt,
                Amount = amount,
                Price = price
            };

            using (var msg = await _client.PostAsJsonAsync(
                $"order?symbol={WebUtility.UrlEncode(kucoinInstrument.Value)}",
                command,
                ct))
            {
                var response = await msg.ReadAsKucoin<CreateLimitOrderResponse>(ct);
                return response.OrderId.GetHexStringToBytes();
            }
        }

        private static KucoinTradeType ConvertTradeType(TradeType tradeType)
        {
            KucoinTradeType ktt;
            switch (tradeType)
            {
                case TradeType.Buy:
                    ktt = KucoinTradeType.Buy;
                    break;
                case TradeType.Sell:
                    ktt = KucoinTradeType.Sell;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tradeType), tradeType, null);
            }

            return ktt;
        }

        public async Task CancelLimitOrder(
            KucoinOrderId orderId,
            CancellationToken ct = default(CancellationToken))
        {
            var cmd = new CancelLimitOrderCommand
            {
                OrderId = orderId.OrderId.ToHexString(),
                TradeType = ConvertTradeType(orderId.TradeType)
            };

            using (var msg = await _client.PostAsJsonAsync(
                $"cancel-order?symbol={orderId.KucoinInstrument.Value}",
                cmd,
                ct))
            {
                await msg.ReadAsKucoin<JToken>(ct);
            }
        }

        public async Task<OrderDetails> GetOrderDetails(
            KucoinOrderId orderId,
            CancellationToken ct = default(CancellationToken))
        {
            using (var msg = await _client.GetAsync(
                $"order/detail?symbol={orderId.KucoinInstrument.Value}" +
                $"&type={ConvertTradeType(orderId.TradeType)}" +
                $"&orderOid={orderId.OrderId.ToHexString()}",
                ct))
            {
                return await msg.ReadAsKucoin<OrderDetails>(ct);
            }
        }


    }
}
