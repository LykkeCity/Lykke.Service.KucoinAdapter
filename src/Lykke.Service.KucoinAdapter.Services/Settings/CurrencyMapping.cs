using System.Collections.Generic;

namespace Lykke.Service.KucoinAdapter.Services.Settings
{
    public sealed class CurrencyMapping
    {
        public IReadOnlyDictionary<string,string> Rename { get; set; }
        public IReadOnlyCollection<string> KnownCurrencies { get; set; }
    }
}
