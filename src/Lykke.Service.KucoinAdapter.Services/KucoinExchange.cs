using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Service.KucoinAdapter.Services.RestApi;
using Lykke.Service.KucoinAdapter.Services.RestApi.Models;
using Lykke.Service.KucoinAdapter.Services.Settings;

namespace Lykke.Service.KucoinAdapter.Services
{
    public sealed class KucoinExchange
    {
        private readonly ILog _log;
        private readonly ApiCredentials _credentials;
        private readonly TimeoutSettings _timeouts;
        private readonly KucoinInstrumentConverter _converter;
        public const string ExchangeName = "kucoin";

        public KucoinExchange(
            ILog log,
            ApiCredentials credentials,
            TimeoutSettings timeouts,
            KucoinInstrumentConverter converter)
        {
            _log = log;
            _credentials = credentials;
            _timeouts = timeouts;
            _converter = converter;
        }

        public IObservable<OrderBook> GetOrderbooks(LykkeInstrument lykkeInstrument, uint limit)
        {
            return Observable.Create<OrderBook>(async (obs, ct) =>
            {
                var client = new RestApiClient(_credentials, _log);
                var kucoinInstrument = _converter.ToKucoinInstrument(lykkeInstrument);

                while (!ct.IsCancellationRequested)
                {
                    try
                    {
                        using (var timed = CancellationTokenSource.CreateLinkedTokenSource(ct))
                        {
                            timed.CancelAfter(_timeouts.RestApiCall);

                            var orderbook = await client.GetOrderbook(kucoinInstrument, limit, timed.Token);
                            obs.OnNext(orderbook.ToOrderbook(lykkeInstrument));
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
