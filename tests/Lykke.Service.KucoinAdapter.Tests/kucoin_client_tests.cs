using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.KucoinAdapter.Services;
using Lykke.Service.KucoinAdapter.Services.RestApi;
using Lykke.Service.KucoinAdapter.Services.RestApi.Models;
using NUnit.Framework;

namespace Lykke.Service.KucoinAdapter.Tests
{
    public class kucoin_client_tests
    {
        private static readonly RestApiClient Client = new RestApiClient(
            new ApiCredentials(
                GetApiKey(),
                GetApiSecret()),

            new LogToConsole());

        private static string GetApiSecret()
        {
            return Environment.GetEnvironmentVariable("KUCOIN_SECRET")
                ?? throw new Exception("Please setup KUCOIN_SECRET environment variable for testing");
        }

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            var knownCurrencies = (await Client.GetSymbols()).SelectMany(x => x.Symbol.Split('-'))
                .Distinct()
                .ToArray();

            _converter = new KucoinInstrumentConverter(
                knownCurrencies,
                new Dictionary<string, string>
                {
                    {"BTC", "1BT"},
                    {"R", "RCURRENCY"}
                });
        }

        private static KucoinInstrumentConverter _converter;

        private static string GetApiKey()
        {
            return Environment.GetEnvironmentVariable("KUCOIN_APIKEY") ??
                throw new Exception("Please setup KUCOIN_APIKEY environment variable for testing");
        }

        [Test]
        public async Task get_balance()
        {
            var balances = (await Client.GetBalance()).ToDictionary(x => x.CoinType);
            Assert.True(balances.Count > 100);
        }

        [Test]
        public async Task get_coins()
        {
            var response = await Client.GetExchangeRateOfCoins();
            Assert.IsNotEmpty(response.Rates["BTC"]);
        }

        public static string[] GetKucoinInstruments()
        {
            var symbols = Task.Run(() => Client.GetSymbols()).Result;
            return symbols.Select(x => x.Symbol).ToArray();
        }

        [Test]
        [TestCaseSource(nameof(GetKucoinInstruments))]
        public void all_instruments_could_be_used_as_lykke_instruments(string instrument)
        {
            var (symbol1, symbol2) = _converter.FromKucoinInstrument(new KucoinInstrument(instrument));
            var lykkeInstrument = _converter.ToLykkeInstrument(symbol1, symbol2);
            var (s1, s2) = _converter.FromLykkeInstrument(lykkeInstrument);
            Assert.AreEqual(instrument, _converter.ToKucoinInstrument(s1, s2).Value);
        }

        [Test]
        public void concrete_conversion_example()
        {
            var (s1, s2) =  _converter.FromKucoinInstrument(new KucoinInstrument("BTC-DENT"));
            Assert.AreEqual("BTC", s1);
            Assert.AreEqual("DENT", s2);

            var lykkeInstrument = _converter.ToLykkeInstrument(s1, s2);
            Assert.AreEqual("1BTDENT", lykkeInstrument.Value);

            var (k1, k2) = _converter.FromLykkeInstrument(lykkeInstrument);
            Assert.AreEqual("BTC", k1);
            Assert.AreEqual("DENT", k2);

            var kucoinInstrument = _converter.ToKucoinInstrument(k1, k2);
            Assert.AreEqual("BTC-DENT", kucoinInstrument.Value);
        }

        [Test]
        public void instrument_parser_should_be_case_insensitive()
        {
            var (s1, s2) = _converter.FromLykkeInstrument(new LykkeInstrument("dentbtc"));
            Assert.AreEqual("DENT", s1);
            Assert.AreEqual("BTC", s2);
        }
    }
}
