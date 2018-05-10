using System.Reactive.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Service.KucoinAdapter.Services;
using NUnit.Framework;

namespace Lykke.Service.KucoinAdapter.Tests
{
    public class orderbook_processing_tests
    {
        [Test]
        public async Task should_NOT_filter_empty_orderbooks()
        {
            var source = Observable.Return(new OrderBook {Asset = "TEST-ASSET"});

            var orderbook = await source.FilterWithReport(new EmptyLog()).FirstOrDefaultAsync();

            Assert.NotNull(orderbook);
            Assert.AreEqual("TEST-ASSET", orderbook.Asset);
            Assert.IsEmpty(orderbook.Asks);
            Assert.IsEmpty(orderbook.Bids);
        }

        [Test]
        public void compare_orderbooks()
        {
            var o1 = new OrderBook {Asset = "asset"};
            var o2 = new OrderBook {Asset = "asset"};
            Assert.AreEqual(o1, o2);
        }

        [Test]
        public async Task should_not_publish_two_empty_orderbooks_consequentially()
        {
            var source = new[]
            {
                new OrderBook {Asset = "asset"},
                new OrderBook {Asset = "asset"}
            }.ToObservable();

            var orderBooks = await source.FilterWithReport(new EmptyLog()).ToArray();

            Assert.NotNull(orderBooks);
            Assert.AreEqual(1, orderBooks.Length);
        }
    }
}
