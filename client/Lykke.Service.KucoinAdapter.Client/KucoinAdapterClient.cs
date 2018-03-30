using System;
using Common.Log;

namespace Lykke.Service.KucoinAdapter.Client
{
    public class KucoinAdapterClient : IKucoinAdapterClient, IDisposable
    {
        private readonly ILog _log;

        public KucoinAdapterClient(string serviceUrl, ILog log)
        {
            _log = log;
        }

        public void Dispose()
        {
            //if (_service == null)
            //    return;
            //_service.Dispose();
            //_service = null;
        }
    }
}
