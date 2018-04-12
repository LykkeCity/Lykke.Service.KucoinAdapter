using System;
using Common;
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

            return new ActiveOrder
            {
                DateTime = arr[0].Value<uint>().FromUnixDateTime(),
                Price = arr[2].Value<decimal>(),
                Amount = arr[3].Value<decimal>(),
                DealAmount = arr[4].Value<decimal>(),
                Id = arr[5].Value<string>().GetHexStringToBytes()
            };
        }
    }
}