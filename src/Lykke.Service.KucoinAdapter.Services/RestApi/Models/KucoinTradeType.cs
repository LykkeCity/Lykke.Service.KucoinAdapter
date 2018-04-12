using System.Runtime.Serialization;

namespace Lykke.Service.KucoinAdapter.Services.RestApi.Models
{
    public enum KucoinTradeType
    {
        [EnumMember(Value = "BUY")] Buy,
        [EnumMember(Value = "SELL")] Sell
    }
}