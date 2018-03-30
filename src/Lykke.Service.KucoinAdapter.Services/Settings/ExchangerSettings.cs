using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.KucoinAdapter.Services.Settings
{
    public sealed class ExchangerSettings
    {
        public bool Enabled { get; set; }
        [AmqpCheck]
        public string ConnectionString { get; set; }
        public string Exchanger { get; set; }
    }
}
