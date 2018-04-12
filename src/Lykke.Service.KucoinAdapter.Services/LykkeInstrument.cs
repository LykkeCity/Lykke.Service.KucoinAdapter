namespace Lykke.Service.KucoinAdapter.Services
{
    public struct LykkeInstrument
    {
        public readonly string Value;

        public LykkeInstrument(string value)
        {
            Value = value;
        }

        public LykkeInstrument(string s1, string s2)
        {
            Value = s1 + s2;
        }
    }
}
