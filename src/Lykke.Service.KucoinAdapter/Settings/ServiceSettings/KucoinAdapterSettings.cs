﻿using System.Collections.Generic;
using Lykke.Common.ExchangeAdapter.Settings;
using Lykke.Service.KucoinAdapter.Services.RestApi.Models;
using Lykke.Service.KucoinAdapter.Services.Settings;
using Newtonsoft.Json;

namespace Lykke.Service.KucoinAdapter.Settings.ServiceSettings
{
    public class KucoinAdapterSettings
    {
        public DbSettings Db { get; set; }

        public OrderbookSettings Orderbooks { get; set; }

        public RabbitMqSettings RabbitMq { get; set; }

        [JsonConverter(typeof(CredentialsConverter<ApiCredentials>))]
        public IReadOnlyDictionary<string, ApiCredentials> Clients { get; set; }
    }
}
