using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Common.ExchangeAdapter.SpotController.Records;
using Lykke.Service.KucoinAdapter.Middleware;
using Lykke.Service.KucoinAdapter.Services;
using Lykke.Service.KucoinAdapter.Services.RestApi.Models;
using Lykke.Service.KucoinAdapter.Settings.ServiceSettings;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.KucoinAdapter.Controllers
{
    [Route("spot")]
    public class SpotController : Controller
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
        public async Task<GetWalletsResponse> GetWallets()
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
        public async Task<GetLimitOrdersResponse> GetOrders(CancellationToken ct)
        {
            return await GetLimitOrdersByInstrument(
                _settings.Orderbooks.Instruments.Select(x => new LykkeInstrument(x)),
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
        public async Task<IActionResult> CreateLimitOrder([FromBody] LimitOrderRequest order)
        {
            var kucoinInstrument = _converter.ToKucoinInstrument(new LykkeInstrument(order.Instrument));

            try
            {
                var orderId = await this.RestApi().CreateLimitOrder(
                    kucoinInstrument,
                    order.Volume,
                    order.Price,
                    order.TradeType);

                var apiId = new KucoinOrderId(kucoinInstrument, orderId, order.TradeType).ToApiId();

                return Ok(new CreateLimitOrderResponse { OrderId = apiId });
            }
            catch (VolumeTooSmallException ex)
            {
                return BadRequest($"volumeTooSmall: {ex.Message}");
            }
        }

        [HttpPost("cancelOrder")]
        [XApiKeyAuth]
        [ProducesResponseType(typeof(CancelLimitOrderResponse), 200)]
        public async Task<IActionResult> CancelLimitOrder(CancelLimitOrderRequest request)
        {
            if (!KucoinOrderId.TryParse(request.OrderId, out var orderId))
            {
                return new NotFoundResult();
            }

            await this.RestApi().CancelLimitOrder(orderId);
            return Ok(request);
        }

        [HttpGet("LimitOrderStatus")]
        [XApiKeyAuth]
        public async Task<IActionResult> GetOrderStatus(string orderId)
        {
            if (!KucoinOrderId.TryParse(orderId, out var koId))
            {
                return new NotFoundResult();
            }

            var orderDetais = await this.RestApi().GetOrderDetails(koId);

            return Ok(ConvertFromFullOrder(orderDetais, koId));
        }

        private OrderModel ConvertFromFullOrder(OrderDetails orderDetais, KucoinOrderId orderId)
        {
            return new OrderModel
            {
                Id = orderId.ToApiId(),
                Symbol = _converter.ToLykkeInstrument(orderId.KucoinInstrument).Value,
                Price = orderDetais.OrderPrice,
                OriginalVolume = orderDetais.DealAmount,
                TradeType = ConvertTradeType(orderDetais.Type),
                Timestamp = new DateTime(1970, 1, 1),
                AvgExecutionPrice =  orderDetais.DealPriceAverage,
                ExecutionStatus = ConvertOrderStatus(orderDetais),
                ExecutedVolume = orderDetais.DealAmount - orderDetais.PendingAmount,
                RemainingAmount = orderDetais.PendingAmount,
            };
        }

        private static OrderStatus ConvertOrderStatus(OrderDetails orderDetais)
        {
            if (orderDetais.PendingAmount != 0) return OrderStatus.Active;
            return OrderStatus.Fill;
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
        public IActionResult ReplaceLimitOrder(OrderDetails _)
        {
            return new StatusCodeResult(501);
        }
    }
}
