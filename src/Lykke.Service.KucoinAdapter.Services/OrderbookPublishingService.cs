using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Common;
using Common.Log;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.KucoinAdapter.Services.Settings;

namespace Lykke.Service.KucoinAdapter.Services
{
    public sealed class OrderbookPublishingService : IStopable
    {
        private readonly OrderbookSettings _orderbookSettings;
        private readonly RabbitMqSettings _rabbitMqSettings;
        private readonly KucoinInstrumentConverter _converter;
        private readonly ILog _log;
        private readonly object _syncRoot = new object();
        private IDisposable _subscription;

        public OrderbookPublishingService(
            OrderbookSettings orderbookSettings,
            RabbitMqSettings rabbitMqSettings,
            KucoinInstrumentConverter converter,
            ILog log)
        {
            _orderbookSettings = orderbookSettings;
            _rabbitMqSettings = rabbitMqSettings;
            _converter = converter;
            _log = log;
        }

        public void Start()
        {
            if (_orderbookSettings.Enabled
#if (DEBUG)
                 && false
#endif
            )
            {
                var exchange = new KucoinExchange(
                    _log,
                    _orderbookSettings.Credentials,
                    _orderbookSettings.Timeouts,
                    _converter);

                var workers = _orderbookSettings.Instruments
                    .Select(x => StartPublishing(new LykkeInstrument(x), exchange));

                _subscription = new CompositeDisposable(workers);
            }
        }

        private IDisposable StartPublishing(
            LykkeInstrument instrument,
            KucoinExchange exchange)
        {
            var orderBooks = exchange
                .GetOrderbooks(instrument, _orderbookSettings.Depth)
                .FilterWithReport(_log)
                .RetryWithBackoff()
                .Publish()
                .RefCount();

            var tickPrices = orderBooks.Select(TickPrice.FromOrderBook).Where(x => x != null).DistinctUntilChanged();

            var orderBooksPublishWorker = ListenAndPublish(orderBooks, _rabbitMqSettings.OrderBooks)
                .Select(_ => Unit.Default)
                .Publish().RefCount();

            var tickPricePublishWorker = ListenAndPublish(tickPrices, _rabbitMqSettings.TickPrices)
                .Select(_ => Unit.Default)
                .Publish().RefCount();

            return Observable.Merge(
                    ReportStatistic(orderBooksPublishWorker, _rabbitMqSettings.OrderBooks.Exchanger, instrument.Value),
                    ReportStatistic(tickPricePublishWorker, _rabbitMqSettings.TickPrices.Exchanger, instrument.Value),
                    tickPricePublishWorker,
                    orderBooksPublishWorker)
                .Subscribe();
        }

        private IObservable<Unit> ReportStatistic(IObservable<Unit> source, string exchanger, string instrument)
        {
            var window = TimeSpan.FromSeconds(60);

            return source
                .Buffer(window)
                .Do(b => _log.WriteInfo(nameof(OrderbookPublishingService), "",
                    $"{b.Count} events for instrument {instrument} published to {exchanger} for last {window}"))
                .Select(_ => Unit.Default);
        }

        private IObservable<T> ListenAndPublish<T>(IObservable<T> source, ExchangerSettings settings)
        {
            return settings.Enabled
                ? Observable.Using(
                        () => StartRmqPublisher<T>(settings),
                        tpPublisher => source.SelectMany(async tp =>
                        {
                            await tpPublisher.ProduceAsync(tp);
                            return tp;
                        }))
                    .RetryWithBackoff()
                : Observable.Never<T>();
        }

        private RabbitMqPublisher<T> StartRmqPublisher<T>(ExchangerSettings exchanger)
        {
            var settings = RabbitMqSubscriptionSettings.CreateForPublisher(
                exchanger.ConnectionString,
                exchanger.Exchanger);

            var connection
                = new RabbitMqPublisher<T>(settings)
                    .SetLogger(_log)
                    .SetSerializer(new JsonMessageSerializer<T>())
                    .SetPublishStrategy(new DefaultFanoutPublishStrategy(settings))
                    .DisableInMemoryQueuePersistence()
                    .Start();

            return connection;
        }

        public void Dispose()
        {
            lock (_syncRoot)
            {
                _subscription?.Dispose();
                _subscription = null;
            }
        }

        public void Stop()
        {
            Dispose();
        }
    }
}
