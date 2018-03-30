using Newtonsoft.Json;

namespace Lykke.Service.KucoinAdapter.Core.Domain.SharedContracts
{
    public sealed class WalletsResponse
    {
        [JsonProperty("wallets")]
        public Wallet[] Wallets { get; set; }
    }
}
