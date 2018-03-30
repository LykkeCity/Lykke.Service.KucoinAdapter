using System;
using System.Collections.Generic;
using System.Linq;
using Common.Log;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.KucoinAdapter.Core.Domain.SharedContracts
{
    public sealed class OrderBook
    {
        private readonly ILog _log;
        private readonly IDictionary<decimal, OrderBookItem> _asks = new Dictionary<decimal, OrderBookItem>();
        private readonly IDictionary<decimal, OrderBookItem> _bids = new Dictionary<decimal, OrderBookItem>();

        public OrderBook()
        {
        }

        public OrderBook(
            ILog log,
            string source,
            string asset,
            DateTime timestamp,
            IEnumerable<OrderBookItem> asks,
            IEnumerable<OrderBookItem> bids)
        {
            _log = log;
            Source = source;
            Asset = asset;
            Timestamp = timestamp;
            _asks = asks.GroupBy(x => x.Price).ToDictionary(x => x.Key,
                x => new OrderBookItem(x.Key, x.Sum(c => c.Volume)));
            _bids = bids.GroupBy(x => x.Price).ToDictionary(x => x.Key,
                x => new OrderBookItem(x.Key, x.Sum(c => c.Volume)));
        }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("asset")]
        public string Asset { get; set; }

        [JsonProperty("timestamp")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Timestamp { get; set; }

        [JsonProperty("asks")]
        public IEnumerable<OrderBookItem> Asks => _asks.Values;

        [JsonProperty("bids")]
        public IEnumerable<OrderBookItem> Bids => _bids.Values;

        public OrderBook Clone(DateTime timestamp)
        {
            return new OrderBook(
                _log,
                Source,
                Asset,
                timestamp,
                Asks.ToArray(),
                Bids.ToArray());
        }

        private void UpdateOrderBook(IDictionary<decimal, OrderBookItem> collection, decimal price, decimal volume)
        {
            var exists = collection.TryGetValue(price, out var current);

            if (volume == 0 && exists)
            {
                collection.Remove(price);
            }
            else
            {
                decimal newVolume;

                if (exists)
                {
                    newVolume = current.Volume + volume;
                }
                else
                {
                    newVolume = volume;
                }

                if (newVolume < 0)
                {
                    Warn($"OrderBook received new update for price-level: {price}, " +
                         $"and market volume for that price equals {newVolume} after update");

                    if (exists)
                    {
                        collection.Remove(price);
                    }
                }
                else if (price == 0)
                {
                    if (exists)
                    {
                        collection.Remove(price);
                    }
                }
                else
                {
                    collection[price] = new OrderBookItem(price, newVolume);
                }
            }
        }

        public void UpdateAsk(decimal price, decimal volume)
        {
            UpdateOrderBook(_asks, price, volume);
        }

        public void UpdateBid(decimal price, decimal volume)
        {
            UpdateOrderBook(_bids, price, volume);
        }

        private void Warn(string message)
        {
            _log.WriteWarning(nameof(OrderBook), nameof(OrderBook), message);
        }
    }
}
