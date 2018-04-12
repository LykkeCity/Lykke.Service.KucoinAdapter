using Newtonsoft.Json;

namespace Lykke.Service.KucoinAdapter.Core.Domain.SharedContracts
{
    public sealed class LimitOrderCreated
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public sealed class ContainsOrderId
    {
        [JsonProperty("orderId")]
        public string OrderId { get; set; }
    }
}
