﻿using System.Collections.Generic;
using Lykke.Service.KucoinAdapter.Services.RestApi.Models;
using Lykke.Service.KucoinAdapter.Services.Settings;

namespace Lykke.Service.KucoinAdapter.Settings.ServiceSettings
{
    public class KucoinAdapterSettings
    {
        public DbSettings Db { get; set; }

        public OrderbookSettings Orderbooks { get; set; }

        public RabbitMqSettings RabbitMq { get; set; }

        public IReadOnlyDictionary<string, ApiCredentials> Clients { get; set; }
    }
}
