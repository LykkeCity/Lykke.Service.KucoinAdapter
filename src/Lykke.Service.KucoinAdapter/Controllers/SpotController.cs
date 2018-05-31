using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Common.ExchangeAdapter.SpotController;
using Lykke.Common.ExchangeAdapter.SpotController.Records;
using Lykke.Logs;
using Lykke.Service.KucoinAdapter.Middleware;
using Lykke.Service.KucoinAdapter.Services;
using Lykke.Service.KucoinAdapter.Services.RestApi;
using Lykke.Service.KucoinAdapter.Services.RestApi.Models;
using Lykke.Service.KucoinAdapter.Services.RestApi.Models.Fails;
using Lykke.Service.KucoinAdapter.Settings.ServiceSettings;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.KucoinAdapter.Controllers
{
    [Route("spot")]
    public class SpotController : Controller // ISpotController
    {
        private readonly KucoinInstrumentConverter _converter;
        private readonly KucoinAdapterSettings _settings;

        public SpotController(
            KucoinInstrumentConverter converter,
            KucoinAdapterSettings settings)
        {
            _converter = converter;
            _settings = settings;
        }

        [HttpGet("getWallets")]
        [XApiKeyAuth]
        public async Task<GetWalletsResponse> GetWalletBalancesAsync()
        {
            var balances = await this.RestApi().GetBalance();

            return new GetWalletsResponse
            {
                Wallets = balances.Select(x => new WalletBalanceModel
                {
                    Asset = _converter.ToLykkeSymbol(x.CoinType),
                    Balance = x.Balance,
                    Reserved = x.FreezeBalance
                }).ToArray()
            };
        }

        [HttpGet("GetLimitOrders")]
        [XApiKeyAuth]
        [ProducesResponseType(typeof(GetLimitOrdersResponse), 200)]
        public async Task<GetLimitOrdersResponse> GetLimitOrdersAsync(CancellationToken ct)
        {
            return await GetLimitOrdersByInstrument(
                _settings.Currencies.SupportedInstruments.Select(x => new LykkeInstrument(x)),
                ct);
        }

        private async Task<GetLimitOrdersResponse> GetLimitOrdersByInstrument(
            IEnumerable<LykkeInstrument> lykkeInstruments,
            CancellationToken ct)
        {
            var result = new List<OrderModel>();

            // we are intentionally querying the exchange consequently
            foreach (var instrument in lykkeInstruments)
            {
                var kucoinInstrument = _converter.ToKucoinInstrument(instrument);
                var byInstrument = await this.RestApi().GetActiveOrders(kucoinInstrument, ct);

                result.AddRange(
                    byInstrument.Sell.Select(x => ConvertFromShortOrder(x,
                        TradeType.Sell,
                        instrument,
                        kucoinInstrument)));
                result.AddRange(
                    byInstrument.Buy.Select(x => ConvertFromShortOrder(x,
                        TradeType.Buy,
                        instrument,
                        kucoinInstrument)));
            }

            return new GetLimitOrdersResponse { Orders = result };
        }

        private static OrderModel ConvertFromShortOrder(
            ActiveOrder order,
            TradeType tradeType,
            LykkeInstrument lykkeInstrument,
            KucoinInstrument instrument)
        {
            return new OrderModel
            {
                Id = new KucoinOrderId(instrument, order.Id, tradeType).ToApiId(),
                Symbol = lykkeInstrument.Value,
                Price = order.Price,
                OriginalVolume = order.Amount,
                TradeType = tradeType,
                Timestamp = order.DateTime,
                AvgExecutionPrice = 0,
                ExecutionStatus = OrderStatus.Active,
                ExecutedVolume = 0,
                RemainingAmount = order.Amount
            };
        }

        [HttpPost("createLimitOrder")]
        [XApiKeyAuth]
        public async Task<OrderIdResponse> CreateLimitOrderAsync([FromBody] LimitOrderRequest order)
        {
            var lykkeInstrument = new LykkeInstrument(order.Instrument);
            EnsureInstrumentIsSupported(lykkeInstrument);

            var kucoinInstrument = _converter.ToKucoinInstrument(lykkeInstrument);

            var orderId = await this.RestApi().CreateLimitOrder(
                kucoinInstrument,
                order.Volume,
                order.Price,
                order.TradeType);

            var apiId = new KucoinOrderId(kucoinInstrument, orderId, order.TradeType).ToApiId();

            return new OrderIdResponse {OrderId = apiId};
        }

        private void EnsureInstrumentIsSupported(LykkeInstrument lykkeInstrument)
        {
            if (!_settings.Currencies.SupportedInstruments.Any(x =>
                lykkeInstrument.Value.Equals(x, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new InvalidInstrumentException(lykkeInstrument.Value);
            }
        }

        [HttpPost("cancelOrder")]
        [XApiKeyAuth]
        public async Task<CancelLimitOrderResponse> CancelLimitOrderAsync([FromBody]CancelLimitOrderRequest request)
        {
            if (!KucoinOrderId.TryParse(request.OrderId, out var orderId))
            {
                throw new InvalidOrderIdException();
            }

            await this.RestApi().CancelLimitOrder(orderId);
            return new CancelLimitOrderResponse{ OrderId = request.OrderId };
        }

        [HttpGet("LimitOrderStatus")]
        [XApiKeyAuth]
        public async Task<OrderModel> LimitOrderStatusAsync(string orderId)
        {
            if (!KucoinOrderId.TryParse(orderId, out var koId))
            {
                throw new InvalidOrderIdException();
            }

            var orderDetais = await this.RestApi().GetOrderDetails(koId);

            var activeOrders = await this.RestApi().GetActiveOrders(koId.KucoinInstrument);

            IReadOnlyCollection<ActiveOrder> orders;

            switch (koId.TradeType)
            {
                case TradeType.Buy:
                    orders = activeOrders.Buy;
                    break;
                case TradeType.Sell:
                    orders = activeOrders.Sell;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return ConvertFromFullOrder(orderDetais, koId, orders.Any(x => x.Id.SequenceEqual(koId.OrderId)));
        }

        [HttpGet("GetInstruments")]
        [XApiKeyAuth]
        public async Task<string[]> GetInstruments()
        {
            return (await this.RestApi().GetSymbols()).Select(x => x.Symbol).ToArray();
        }

        private OrderModel ConvertFromFullOrder(OrderDetails orderDetais, KucoinOrderId orderId, bool isActive)
        {
            return new OrderModel
            {
                Id = orderId.ToApiId(),
                Symbol = _converter.ToLykkeInstrument(orderId.KucoinInstrument).Value,
                Price = orderDetais.OrderPrice,
                OriginalVolume = orderDetais.DealAmount + orderDetais.PendingAmount,
                TradeType = ConvertTradeType(orderDetais.Type),
                Timestamp = orderDetais.CreatedAt.FromKuckoinDateTime(),
                AvgExecutionPrice =  orderDetais.DealPriceAverage,
                ExecutionStatus = ConvertOrderStatus(orderDetais, isActive),
                ExecutedVolume = orderDetais.DealAmount,
                RemainingAmount = orderDetais.PendingAmount,
            };
        }

        private static OrderStatus ConvertOrderStatus(OrderDetails orderDetais, bool isActive)
        {
            if (isActive) return OrderStatus.Active;

            return orderDetais.DealAmount == 0 ? OrderStatus.Canceled : OrderStatus.Fill;
        }

        private TradeType ConvertTradeType(KucoinTradeType orderDetaisType)
        {
            switch (orderDetaisType)
            {
                case KucoinTradeType.Buy:
                    return TradeType.Buy;
                case KucoinTradeType.Sell:
                    return TradeType.Sell;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orderDetaisType), orderDetaisType, null);
            }
        }

        [HttpPost("replaceLimitOrder")]
        [XApiKeyAuth]
        [ProducesResponseType(typeof(OrderIdResponse), 200)]
        public IActionResult ReplaceLimitOrder([FromBody]ReplaceLimitOrderRequest _)
        {
            return new StatusCodeResult(501);
        }
    }
}
