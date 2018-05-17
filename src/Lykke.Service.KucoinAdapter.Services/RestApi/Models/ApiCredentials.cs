using System.Security.Cryptography;
using System.Text;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.KucoinAdapter.Services.RestApi.Models
{
    public sealed class ApiCredentials
    {
        public ApiCredentials()
        {
        }

        [Optional]
        public string InternalApiKey { get; set; }
        public string ApiKey { get; set; }
        public string Secret { get; set; }

        public ApiCredentials(string apiKey, string secret)
        {
            ApiKey = apiKey;
            Secret = secret;
        }

        public HMACSHA256 CreateHmac()
        {
            return new HMACSHA256(Encoding.UTF8.GetBytes(Secret));
        }
    }
}
