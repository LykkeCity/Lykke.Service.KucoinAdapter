using Newtonsoft.Json;

namespace Lykke.Service.KucoinAdapter.Services.RestApi.Models
{
    public sealed class GetSymbolsElement
    {
        [JsonProperty("coinType")]
        public string CoinType { get; set; }

        [JsonProperty("trading")]
        public bool Trading { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("lastDealPrice")]
        public decimal LastDealPrice { get; set; }

        [JsonProperty("buy")]
        public decimal Buy { get; set; }

        [JsonProperty("sell")]
        public decimal Sell { get; set; }

        [JsonProperty("change")]
        public decimal Change { get; set; }

        [JsonProperty("coinTypePair")]
        public string CoinTypePair { get; set; }

        [JsonProperty("sort")]
        public long Sort { get; set; }

        [JsonProperty("feeRate")]
        public decimal FeeRate { get; set; }

        [JsonProperty("volValue")]
        public decimal VolValue { get; set; }

        [JsonProperty("high")]
        public decimal? High { get; set; }

        [JsonProperty("datetime")]
        public long Datetime { get; set; }

        [JsonProperty("vol")]
        public decimal Vol { get; set; }

        [JsonProperty("low")]
        public decimal? Low { get; set; }

        [JsonProperty("changeRate")]
        public decimal ChangeRate { get; set; }

        [JsonProperty("stick")]
        public bool Stick { get; set; }

        [JsonProperty("fav")]
        public bool Fav { get; set; }
    }
}
