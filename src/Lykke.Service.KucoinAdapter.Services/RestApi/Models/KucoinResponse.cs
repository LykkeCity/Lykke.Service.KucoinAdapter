using System;
using Lykke.Service.KucoinAdapter.Services.RestApi.Models.Fails;
using Newtonsoft.Json;

namespace Lykke.Service.KucoinAdapter.Services.RestApi.Models
{
    public sealed class KucoinResponse<T>
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("msg")]
        public string Message { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }

        public void EnsureNoError()
        {
            if (!Success)
            {
                {
                    if (VolumeTooSmallException.TryParse(Message, out var vtsEx))
                    {
                        throw vtsEx;
                    }

                    if (NoBalanceException.TryParse(Code, out var nbEx))
                    {
                        throw nbEx;
                    }

                    throw new Exception($"Erroneous response: [{Code}] {Message}");
                }
            }
        }
    }
}
