using System.Security.Cryptography;
using System.Text;

namespace Lykke.Service.KucoinAdapter.Services.RestApi.Models
{
    public sealed class ApiCredentials
    {
        public ApiCredentials()
        {
        }

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
