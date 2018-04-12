using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.KucoinAdapter.Services.RestApi.Models
{
    public sealed class CancelLimitOrderCommand
    {
        [JsonProperty("orderId")]
        public string OrderId { get; set; }

        [JsonProperty("type"), JsonConverter(typeof(StringEnumConverter))]
        public KucoinTradeType TradeType { get; set; }
    }
}
