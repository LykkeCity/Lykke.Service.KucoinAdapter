using System;
using Common;
using Lykke.Service.KucoinAdapter.Core.Domain.SharedContracts;

namespace Lykke.Service.KucoinAdapter.Services.RestApi.Models
{
    public struct KucoinOrderId
    {
        public KucoinOrderId(KucoinInstrument kucoinInstrument, byte[] orderId, TradeType tradeType)
        {
            KucoinInstrument = kucoinInstrument;
            OrderId = orderId;
            TradeType = tradeType;
        }

        public readonly KucoinInstrument KucoinInstrument;
        public readonly byte[] OrderId;
        public readonly TradeType TradeType;

        public string ToApiId()
        {
            return $"{KucoinInstrument.Value.Replace(":", "")}:{TradeType:G}:{OrderId.ToHexString()}";
        }

        public static KucoinOrderId? Parse(string source)
        {
            var parts = source.Split(":", 3);

            if (parts.Length != 3) return null;

            try
            {
                var arr = Utils.HexToArray(parts[2]);

                return new KucoinOrderId(new KucoinInstrument(parts[0]), arr, Enum.Parse<TradeType>(parts[1], true));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool TryParse(string source, out KucoinOrderId orderId)
        {
            var parsed = Parse(source);
            if (parsed.HasValue)
            {
                orderId = parsed.Value;
                return true;
            }

            orderId = new KucoinOrderId();

            return false;
        }
    }
}
