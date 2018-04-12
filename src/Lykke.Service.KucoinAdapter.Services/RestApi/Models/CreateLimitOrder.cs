using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.KucoinAdapter.Services.RestApi.Models
{
    public sealed class CreateLimitOrder
    {
        [JsonProperty("type"), JsonConverter(typeof(StringEnumConverter))]
        public KucoinTradeType TradeType { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }
}