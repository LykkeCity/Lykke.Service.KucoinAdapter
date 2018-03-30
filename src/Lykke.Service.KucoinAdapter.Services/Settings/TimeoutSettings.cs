using System;

namespace Lykke.Service.KucoinAdapter.Services.Settings
{
    public sealed class TimeoutSettings
    {
        public TimeSpan BetweenApiCalls { get; set; }
        public TimeSpan RestApiCall { get; set; }
    }
}