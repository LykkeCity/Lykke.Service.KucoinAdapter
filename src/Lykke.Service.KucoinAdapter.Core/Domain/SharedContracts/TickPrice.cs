using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.KucoinAdapter.Core.Domain.SharedContracts
{
    public class TickPrice
    {
        private bool Equals(TickPrice other)
        {
            return string.Equals(Source, other.Source)
                   && string.Equals(Asset, other.Asset)
                   && Ask == other.Ask && Bid == other.Bid;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TickPrice) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Source != null ? Source.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Asset != null ? Asset.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Ask.GetHashCode();
                hashCode = (hashCode * 397) ^ Bid.GetHashCode();
                return hashCode;
            }
        }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("asset")]
        public string Asset { get; set; }

        [JsonProperty("timestamp")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Timestamp { get; set; }

        [JsonProperty("ask")]
        public decimal Ask { get; set; }

        [JsonProperty("bid")]
        public decimal Bid { get; set; }

        public static TickPrice FromOrderBook(OrderBook orderBook)
        {
            if (!orderBook.Bids.Any() || !orderBook.Asks.Any()) return null;

            return new TickPrice
            {
                Source = orderBook.Source,
                Asset = orderBook.Asset,
                Timestamp = orderBook.Timestamp,
                Ask = orderBook.Asks.Min(x => x.Price),
                Bid = orderBook.Bids.Max(x => x.Price)
            };
        }
    }
}
