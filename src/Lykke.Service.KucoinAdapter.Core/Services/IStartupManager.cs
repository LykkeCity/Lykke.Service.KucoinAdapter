using System.Threading.Tasks;

namespace Lykke.Service.KucoinAdapter.Core.Services
{
    public interface IStartupManager
    {
        Task StartAsync();
    }
}