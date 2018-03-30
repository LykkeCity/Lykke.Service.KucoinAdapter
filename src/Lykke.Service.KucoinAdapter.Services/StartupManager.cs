using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.KucoinAdapter.Core.Services;

namespace Lykke.Service.KucoinAdapter.Services
{
    // NOTE: Sometimes, startup process which is expressed explicitly is not just better, 
    // but the only way. If this is your case, use this class to manage startup.
    // For example, sometimes some state should be restored before any periodical handler will be started, 
    // or any incoming message will be processed and so on.
    // Do not forget to remove As<IStartable>() and AutoActivate() from DI registartions of services, 
    // which you want to startup explicitly.

    public class StartupManager : IStartupManager
    {
        private readonly ILog _log;
        private readonly OrderbookPublishingService _publishingService;

        public StartupManager(ILog log, OrderbookPublishingService publishingService)
        {
            _log = log;
            _publishingService = publishingService;
        }

        public async Task StartAsync()
        {
            // TODO: Implement your startup logic here. Good idea is to log every step

            _publishingService.Start();

            await Task.CompletedTask;
        }
    }
}
