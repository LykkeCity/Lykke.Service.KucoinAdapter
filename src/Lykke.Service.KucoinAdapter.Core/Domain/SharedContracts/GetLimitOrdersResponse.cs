using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lykke.Service.KucoinAdapter.Core.Domain.SharedContracts
{
    public sealed class GetLimitOrdersResponse
    {
        [JsonProperty("Orders")]
        public IReadOnlyCollection<Order> Orders { get; set; }
    }
}