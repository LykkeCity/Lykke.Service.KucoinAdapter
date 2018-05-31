using System.Collections.Generic;

namespace Lykke.Service.KucoinAdapter.Services.Settings
{
    public sealed class CurrencySettings
    {
        public IReadOnlyCollection<string> SupportedInstruments { get; set; }
        public IReadOnlyCollection<string> KnownCurrencies { get; set; }
        public IReadOnlyDictionary<string,string> Rename { get; set; }
    }
}
