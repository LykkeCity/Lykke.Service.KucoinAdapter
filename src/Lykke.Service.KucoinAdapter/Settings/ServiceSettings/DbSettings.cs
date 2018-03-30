using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.KucoinAdapter.Settings.ServiceSettings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}
