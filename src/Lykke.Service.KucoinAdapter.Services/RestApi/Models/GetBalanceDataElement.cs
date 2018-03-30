using Newtonsoft.Json;

namespace Lykke.Service.KucoinAdapter.Services.RestApi.Models
{
    public sealed class GetBalanceDataElement
    {
        [JsonProperty("coinType")]
        public string CoinType { get; set; }

        [JsonProperty("balanceStr")]
        public string BalanceStr { get; set; }

        [JsonProperty("freezeBalance")]
        public decimal FreezeBalance { get; set; }

        [JsonProperty("balance")]
        public decimal Balance { get; set; }

        [JsonProperty("freezeBalanceStr")]
        public string FreezeBalanceStr { get; set; }
    }
}
