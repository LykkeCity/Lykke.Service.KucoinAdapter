using Lykke.Service.KucoinAdapter.Settings.ServiceSettings;
using Lykke.Service.KucoinAdapter.Settings.SlackNotifications;

namespace Lykke.Service.KucoinAdapter.Settings
{
    public class AppSettings
    {
        public KucoinAdapterSettings KucoinAdapterService { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}
