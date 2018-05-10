using System.Reactive.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.KucoinAdapter.Core.Domain.SharedContracts;
using Lykke.Service.KucoinAdapter.Services;
using NUnit.Framework;

namespace Lykke.Service.KucoinAdapter.Tests
{
    public class orderbook_processing_tests
    {
        [Test]
        public async Task should_NOT_filter_empty_orderbooks()
        {
            var source = Observable.Return(new OrderBook { Asset = "TEST-ASSET" });

            var orderbook = await source.FilterWithReport(new EmptyLog()).FirstOrDefaultAsync();

            Assert.NotNull(orderbook);
            Assert.AreEqual("TEST-ASSET", orderbook.Asset);
            Assert.IsEmpty(orderbook.Asks);
            Assert.IsEmpty(orderbook.Bids);
        }

    }
}