using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lykke.Service.KucoinAdapter.Services.RestApi.Models
{
    public class ExchangeRateOfCoins
    {
        [JsonProperty("currencies")]
        public string[][] Currencies { get; set; }

        [JsonProperty("rates")]
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, decimal>> Rates { get; set; }
    }
}
