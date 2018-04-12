using System.Runtime.Serialization;

namespace Lykke.Service.KucoinAdapter.Core.Domain.SharedContracts
{
    public enum TradeType
    {
        [EnumMember(Value = "Buy")] Buy,
        [EnumMember(Value = "Sell")] Sell
    }
}