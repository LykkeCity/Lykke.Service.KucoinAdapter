using System;
using System.Reactive.Linq;
using Common.Log;
using Lykke.Common.ExchangeAdapter.Contracts;

namespace Lykke.Service.KucoinAdapter.Services
{
    public static class OrderBookProcessPipelines
    {
        private static (bool, string, OrderBook) FromTryGetPattern(OrderBook orderBook)
        {
            return (orderBook.TryDetectNegativeSpread(out var error), error, orderBook);
        }

        public static IObservable<OrderBook> DetectNegativeSpread(this IObservable<OrderBook> source, ILog log)
        {
            return source.Select(FromTryGetPattern)
                .GroupBy(x => x.Item1)
                .Select(group =>
                {
                    return group.Key
                        ? ReportNegativeSpread(log, group.Select(x => x.Item2), TimeSpan.FromSeconds(5))
                        : group.Select(x => x.Item3);
                })
                .Merge()
                .Where(x => x != null);
        }

        private static IObservable<OrderBook> ReportNegativeSpread(
            ILog log,
            IObservable<string> group,
            TimeSpan errorThrottlingPeriod)
        {
            return group
                .Buffer(errorThrottlingPeriod)
                .Do(x =>
                {
                    if (x.Count == 1)
                    {
                        var error = x[0];
                        log.WriteInfo(nameof(OrderBookProcessPipelines), "", error);
                    }
                    else if (x.Count > 0)
                    {
                        log.WriteInfo(nameof(OrderBookProcessPipelines), "",
                            $"{x.Count} orderbooks with negative spread for last: {errorThrottlingPeriod}");
                    }
                })
                .Select(_ => (OrderBook)null);
        }

        public static IObservable<OrderBook> FilterWithReport(this IObservable<OrderBook> source, ILog log)
        {
            return source
                .DetectNegativeSpread(log)
                .DistinctUntilChanged();
        }
    }
}
