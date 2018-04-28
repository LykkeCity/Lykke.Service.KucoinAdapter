using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.KucoinAdapter.Core.Domain.SharedContracts;
using Lykke.Service.KucoinAdapter.Services.RestApi;
using Lykke.Service.KucoinAdapter.Services.Settings;

namespace Lykke.Service.KucoinAdapter.Services
{
    public sealed class KucoinExchange
    {
        private readonly CurrencyMapping _currencyMapping;
        private readonly ILog _log;
        private readonly ApiCredentials _credentials;
        private readonly TimeoutSettings _timeouts;
        public const string ExchangeName = "kucoin";

        public KucoinExchange(
            CurrencyMapping currencyMapping,
            ILog log,
            ApiCredentials credentials,
            TimeoutSettings timeouts)
        {
            _currencyMapping = currencyMapping;
            _log = log;
            _credentials = credentials;
            _timeouts = timeouts;
        }

        public IObservable<OrderBook> GetOrderbooks(string lykkeInstrument)
        {
            return Observable.Create<OrderBook>(async (obs, ct) =>
            {
                var converter = new KucoinInstrument(_currencyMapping.KnownCurrencies, _currencyMapping.Rename);

                var client = new RestApiClient(_credentials, _log);
                var (s1, s2) = converter.FromLykkeInstrument(lykkeInstrument);
                var kucoinInstrument = converter.ToKucoinInstrument(s1, s2);

                while (!ct.IsCancellationRequested)
                {
                    try
                    {
                        using (var timed = CancellationTokenSource.CreateLinkedTokenSource(ct))
                        {
                            timed.CancelAfter(_timeouts.RestApiCall);

                            var orderbook = await client.GetOrderbook(kucoinInstrument, timed.Token);
                            obs.OnNext(orderbook.ToOrderbook(_log, lykkeInstrument));
                        }
                    }
                    catch (Exception ex)
                    {
                        obs.OnError(ex);
                    }

                    await Task.Delay(_timeouts.BetweenApiCalls, ct);
                }
            });
        }
    }
}
