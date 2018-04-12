using System.Runtime.Serialization;

namespace Lykke.Service.KucoinAdapter.Core.Domain.SharedContracts
{
    public enum OrderStatus
    {
        [EnumMember(Value = "Active")] Active,
        [EnumMember(Value = "Canceled")] Canceled,
        [EnumMember(Value = "Fill")] Fill
    }
}