using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.KucoinAdapter.Services.RestApi.Models
{
    public sealed class ActiveOrders
    {
        public IReadOnlyCollection<ActiveOrder> Buy { get; }
        public IReadOnlyCollection<ActiveOrder> Sell { get; }

        public ActiveOrders(ActiveOrdersListResponse response)
        {
            Buy = response.Buy.Select(ActiveOrder.FromJArray).ToArray();
            Sell = response.Sell.Select(ActiveOrder.FromJArray).ToArray();
        }
    }
}