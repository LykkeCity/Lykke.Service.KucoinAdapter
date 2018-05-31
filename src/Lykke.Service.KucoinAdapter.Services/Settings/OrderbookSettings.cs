using System.Collections.Generic;
using Lykke.Service.KucoinAdapter.Services.RestApi.Models;

namespace Lykke.Service.KucoinAdapter.Services.Settings
{
    public sealed class OrderbookSettings
    {
        public bool Enabled { get; set; }
        public ApiCredentials Credentials { get; set; }
        public TimeoutSettings Timeouts { get; set; }
        public uint Depth { get; set; }
    }
}
