using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.KucoinAdapter.Core.Domain.SharedContracts
{
    public sealed class CreateLimitOrderCommand
    {
        [JsonProperty("Instrument")]
        public string Instrument { get; set; }
        [JsonProperty("Price")]
        public decimal Price { get; set; }
        [JsonProperty("Amount")]
        public decimal Amount { get; set; }
        [JsonProperty("TradeType"), JsonConverter(typeof(StringEnumConverter))]
        public TradeType TradeType { get; set; }
    }
}
