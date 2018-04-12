using Newtonsoft.Json;

namespace Lykke.Service.KucoinAdapter.Services.RestApi.Models
{
    public sealed class OrderDetailsDeal
    {
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("dealValue")]
        public decimal DealValue { get; set; }

        [JsonProperty("fee")]
        public decimal Fee { get; set; }

        [JsonProperty("dealPrice")]
        public decimal DealPrice { get; set; }

        [JsonProperty("feeRate")]
        public decimal FeeRate { get; set; }
    }
}
