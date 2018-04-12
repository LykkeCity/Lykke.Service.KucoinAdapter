using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lykke.Service.KucoinAdapter.Services.RestApi.Models
{
    public sealed class ActiveOrdersListResponse
    {
        [JsonProperty("SELL")]
        public IReadOnlyCollection<JArray> Sell { get; set; }

        [JsonProperty("BUY")]
        public IReadOnlyCollection<JArray> Buy { get; set; }
    }
}