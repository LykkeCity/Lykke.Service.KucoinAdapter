using Newtonsoft.Json;

namespace Lykke.Service.KucoinAdapter.Services.RestApi.Models
{
    public sealed class CreateLimitOrderResponse
    {
        [JsonProperty("orderOid")]
        public string OrderId { get; set; }
    }
}