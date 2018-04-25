using System.Linq;

namespace Lykke.Service.KucoinAdapter.Core.Domain.SharedContracts
{
    public static class OrderBookExtensions
    {
        public static (bool, string, OrderBook) DetectNegativeSpread(this OrderBook orderBook)
        {
            if (orderBook.Asks.Any() && orderBook.Bids.Any())
            {
                var bestAsk = orderBook.Asks.Min(ob => ob.Price);
                var bestBid = orderBook.Bids.Max(ob => ob.Price);
                if (bestAsk < bestBid)
                {
                    var info = $"Orderbook for asset {orderBook.Asset} has negative spread, " +
                               $"minAsk: {bestAsk}, " +
                               $"maxBid: {bestBid}," +
                               $"spread: {bestAsk - bestBid}";
                    return (true, info, orderBook);
                }
            }

            return (false, null, orderBook);
        }
    }
}
