namespace Lykke.Service.KucoinAdapter.Services.Settings
{
    public sealed class RabbitMqSettings
    {
        public ExchangerSettings OrderBooks { get; set; }
        public ExchangerSettings TickPrices { get; set; }
    }
}
