using System;
using System.Linq;
using Common.Log;
using Lykke.Service.KucoinAdapter.Core.Domain.SharedContracts;
using Newtonsoft.Json;

namespace Lykke.Service.KucoinAdapter.Services.RestApi.Models
{
    public sealed class KucoinOrderbook
    {
        private decimal[][] _buy;
        private decimal[][] _sell;

        [JsonProperty("BUY")]
        public decimal[][] Buy
        {
            get => _buy ?? new decimal[0][];
            set => _buy = value;
        }

        [JsonProperty("SELL")]
        public decimal[][] Sell
        {
            get => _sell ?? new decimal[0][];
            set => _sell = value;
        }

        public OrderBook ToOrderbook(ILog log, string asset)
        {
            return new OrderBook(log, KucoinExchange.ExchangeName, asset, DateTime.UtcNow,
                bids: Buy.Select(x => new OrderBookItem(x[0], x[1])),
                asks: Sell.Select(x => new OrderBookItem(x[0], x[1])));
        }
    }
}
