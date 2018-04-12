using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.KucoinAdapter.Services
{
    public sealed class KucoinInstrumentConverter
    {
        private readonly IReadOnlyDictionary<string, string> _rename;
        private readonly IReadOnlyDictionary<string, string> _inverted;

        public KucoinInstrumentConverter(
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

        public (string, string) FromLykkeInstrument(LykkeInstrument lykkeInstrument)
        {
            var lykkeSymbol1 = _lykkeCurrencies.FirstOrDefault(lykkeInstrument.Value.StartsWith);

            if (lykkeSymbol1 == null)
            {

                var lykkeSymbol2 = _lykkeCurrencies.FirstOrDefault(lykkeInstrument.Value.EndsWith);

                if (lykkeSymbol2 == null)
                {
                    throw new ArgumentException($"{lykkeInstrument.Value} cannot be parsed as pair of known currencies");
                }

                var startOfTheInstrument = lykkeInstrument.Value.Substring(0, lykkeInstrument.Value.Length - lykkeSymbol2.Length);

                return (ToKucoinSymbol(startOfTheInstrument), ToKucoinSymbol(lykkeSymbol2));
            }

            var restLykkeSymbol2 = lykkeInstrument.Value.Substring(lykkeSymbol1.Length);

            return (ToKucoinSymbol(lykkeSymbol1), ToKucoinSymbol(restLykkeSymbol2));
        }

        public (string, string) FromKucoinInstrument(KucoinInstrument instrument)
        {
            var parts = instrument.Value.Split('-');

            if (parts.Length != 2)
            {
                throw new ArgumentException("Cannot be parsed as pair of symbols", nameof(instrument));
            }

            return (parts[0], parts[1]);
        }

        public LykkeInstrument ToLykkeInstrument(string symbol1, string symbol2)
        {
            return new LykkeInstrument(ToLykkeSymbol(symbol1), ToLykkeSymbol(symbol2));
        }

        public LykkeInstrument ToLykkeInstrument(KucoinInstrument kucoinInstrument)
        {
            var (s1, s2) = FromKucoinInstrument(kucoinInstrument);
            return ToLykkeInstrument(ToLykkeSymbol(s1), ToLykkeSymbol(s2));
        }

        public string ToLykkeSymbol(string symbol)
        {
            return _rename.TryGetValue(symbol, out var lykkeSymbol) ? lykkeSymbol : symbol;
        }

        public string ToKucoinSymbol(string symbol)
        {
            return _inverted.TryGetValue(symbol, out var kucoinSymbol) ? kucoinSymbol : symbol;
        }

        public KucoinInstrument ToKucoinInstrument(string symbol1, string symbol2)
        {
            return new KucoinInstrument(symbol1, symbol2);
        }

        public KucoinInstrument ToKucoinInstrument(LykkeInstrument lykkeInstrument)
        {
            var (s1, s2) = FromLykkeInstrument(lykkeInstrument);
            return ToKucoinInstrument(s1, s2);
        }
    }
}
