using System;
using Autofac;
using Common.Log;

namespace Lykke.Service.KucoinAdapter.Client
{
    public static class AutofacExtension
    {
        public static void RegisterKucoinAdapterClient(this ContainerBuilder builder, string serviceUrl, ILog log)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (serviceUrl == null) throw new ArgumentNullException(nameof(serviceUrl));
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            builder.RegisterType<KucoinAdapterClient>()
                .WithParameter("serviceUrl", serviceUrl)
                .As<IKucoinAdapterClient>()
                .SingleInstance();
        }

        public static void RegisterKucoinAdapterClient(this ContainerBuilder builder, KucoinAdapterServiceClientSettings settings, ILog log)
        {
            builder.RegisterKucoinAdapterClient(settings?.ServiceUrl, log);
        }
    }
}
