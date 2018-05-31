using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common;
using Common.Log;
using Lykke.Service.KucoinAdapter.Core.Services;
using Lykke.Service.KucoinAdapter.Settings.ServiceSettings;
using Lykke.Service.KucoinAdapter.Services;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.KucoinAdapter.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<KucoinAdapterSettings> _settings;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public ServiceModule(IReloadingManager<KucoinAdapterSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            // TODO: Do not register entire settings in container, pass necessary settings to services which requires them
            // ex:
            //  builder.RegisterType<QuotesPublisher>()
            //      .As<IQuotesPublisher>()
            //      .WithParameter(TypedParameter.From(_settings.CurrentValue.QuotesPublication))

            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            var kucoinAdapterSettings = _settings.CurrentValue;

            builder.RegisterType<KucoinInstrumentConverter>()
                .WithParameter("knownCurrencies", kucoinAdapterSettings.Currencies.KnownCurrencies)
                .WithParameter("rename", kucoinAdapterSettings.Currencies.Rename)
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<OrderbookPublishingService>()
                .WithParameter("orderbookSettings", kucoinAdapterSettings.Orderbooks)
                .WithParameter("rabbitMqSettings", kucoinAdapterSettings.RabbitMq)
                .WithParameter("currencySettings", kucoinAdapterSettings.Currencies)
                .As<IStopable>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterInstance(kucoinAdapterSettings)
                .SingleInstance();

            // TODO: Add your dependencies here

            builder.Populate(_services);
        }
    }
}
