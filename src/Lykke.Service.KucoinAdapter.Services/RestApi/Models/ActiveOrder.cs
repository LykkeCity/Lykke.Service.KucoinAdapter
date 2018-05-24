using System;
using Common;
using Lykke.Common.ExchangeAdapter;
using Newtonsoft.Json.Linq;

namespace Lykke.Service.KucoinAdapter.Services.RestApi.Models
{
    public sealed class ActiveOrder
    {
        public DateTime DateTime { get; private set; }
        public decimal Price { get; private set; }
        public decimal Amount { get; private set; }
        public decimal DealAmount { get; private set; }
        public byte[] Id { get; private set; }

        public static ActiveOrder FromJArray(JArray arr)
        {
            if (arr.Count != 6)
            {
                throw new ArgumentException($"Expected JArray of 6 elements, got  {arr.Count}", nameof(arr));
            }

            var dt = arr[0].Value<long>().FromKuckoinDateTime();
            var price = arr[2].Value<decimal>();
            var amount = arr[3].Value<decimal>();
            var dealAmount = arr[4].Value<decimal>();
            var id = arr[5].Value<string>().GetHexStringToBytes();

            return new ActiveOrder
            {
                DateTime = dt,
                Price = price,
                Amount = amount,
                DealAmount = dealAmount,
                Id = id
            };
        }
    }
}
