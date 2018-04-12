namespace Lykke.Service.KucoinAdapter.Services
{
    public struct KucoinInstrument
    {
        public readonly string Value;

        public KucoinInstrument(string value)
        {
            Value = value;
        }

        public KucoinInstrument(string symbol1, string symbol2)
        {
            Value = $"{symbol1}-{symbol2}";
        }
    }
}
