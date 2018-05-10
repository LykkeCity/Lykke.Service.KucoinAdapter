using System;
using System.Reactive.Linq;
using Common.Log;
using Lykke.Service.KucoinAdapter.Core.Domain.SharedContracts;

namespace Lykke.Service.KucoinAdapter.Services
{
    public static class OrderBookProcessPipelines
    {
        public static IObservable<OrderBook> DetectNegativeSpread(this IObservable<OrderBook> source, ILog log)
        {
            return source.Select(x => OrderBookExtensions.DetectNegativeSpread(x))
                .Do(x => ReportNegativeSpread(x.Item1, x.Item2, log),
                    err => log.WriteInfo(nameof(OrderbookPublishingService), "orderbooks", err.ToString()))
                .Where(x => !x.Item1).Select(x => x.Item3);
        }

        private static void ReportNegativeSpread(bool hasNegativeSpread, string error, ILog log)
        {
            if (hasNegativeSpread) log.WriteInfo(nameof(OrderbookPublishingService), "", error);
        }

        public static IObservable<OrderBook> FilterWithReport(this IObservable<OrderBook> source, ILog log)
        {
            return source
                .DetectNegativeSpread(log);
        }
    }
}
