using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.KucoinAdapter.Services
{
    public sealed class KucoinInstrument
    {
        private readonly IReadOnlyDictionary<string, string> _rename;
        private readonly IReadOnlyDictionary<string, string> _inverted;

        public KucoinInstrument(
            IEnumerable<string> knownCurrencies,
            IReadOnlyDictionary<string, string> rename = null)
        {
            if (knownCurrencies == null) throw new ArgumentNullException(nameof(knownCurrencies));

            _rename = rename ?? new Dictionary<string, string>();
            _inverted = _rename.ToDictionary(x => x.Value, x => x.Key);
            var kucoinCurrencies = knownCurrencies.Distinct().ToArray();
            _lykkeCurrencies = kucoinCurrencies.Select(ToLykkeSymbol).OrderByDescending(x => x.Length).ToArray();
        }

        private readonly string[] _lykkeCurrencies;

        public (string, string) FromLykkeInstrument(string instrument)
        {
            var lykkeSymbol1 = _lykkeCurrencies.FirstOrDefault(instrument.StartsWith);

            if (lykkeSymbol1 == null)
            {

                var lykkeSymbol2 = _lykkeCurrencies.FirstOrDefault(instrument.EndsWith);

                if (lykkeSymbol2 == null)
                {
                    throw new ArgumentException($"{instrument} cannot be parsed as pair of known currencies");
                }

                var startOfTheInstrument = instrument.Substring(0, instrument.Length - lykkeSymbol2.Length);

                return (ToKucoinSymbol(startOfTheInstrument), ToKucoinSymbol(lykkeSymbol2));
            }

            var restLykkeSymbol2 = instrument.Substring(lykkeSymbol1.Length);

            return (ToKucoinSymbol(lykkeSymbol1), ToKucoinSymbol(restLykkeSymbol2));
        }

        public (string, string) FromKucoinInstrument(string instrument)
        {
            var parts = instrument.Split('-');

            if (parts.Length != 2)
            {
                throw new ArgumentException("Cannot be parsed as pair of symbols", nameof(instrument));
            }

            return (parts[0], parts[1]);
        }

        public string ToLykkeInstrument(string symbol1, string symbol2)
        {
            return ToLykkeSymbol(symbol1) + ToLykkeSymbol(symbol2);
        }

        private string ToLykkeSymbol(string symbol)
        {
            return _rename.TryGetValue(symbol, out var lykkeSymbol) ? lykkeSymbol : symbol;
        }

        private string ToKucoinSymbol(string symbol)
        {
            return _inverted.TryGetValue(symbol, out var kucoinSymbol) ? kucoinSymbol : symbol;
        }

        public string ToKucoinInstrument(string symbol1, string symbol2)
        {
            return $"{symbol1}-{symbol2}";
        }
    }
}
