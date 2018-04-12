using Newtonsoft.Json;

namespace Lykke.Service.KucoinAdapter.Services.RestApi.Models
{
    public sealed class OrderDetails
    {
        [JsonProperty("coinType")]
        public string CoinType { get; set; }

        [JsonProperty("dealValueTotal")]
        public decimal DealValueTotal { get; set; }

        [JsonProperty("dealPriceAverage")]
        public decimal DealPriceAverage { get; set; }

        [JsonProperty("feeTotal")]
        public decimal FeeTotal { get; set; }

        [JsonProperty("userOid")]
        public string UserOid { get; set; }

        [JsonProperty("dealAmount")]
        public decimal DealAmount { get; set; }

        [JsonProperty("dealOrders")]
        public DealOrders DealOrders { get; set; }

        [JsonProperty("coinTypePair")]
        public string CoinTypePair { get; set; }

        [JsonProperty("orderPrice")]
        public decimal OrderPrice { get; set; }

        [JsonProperty("type")]
        public KucoinTradeType Type { get; set; }

        [JsonProperty("orderOid")]
        public string OrderOid { get; set; }

        [JsonProperty("pendingAmount")]
        public decimal PendingAmount { get; set; }
    }
}
