using Newtonsoft.Json;

namespace Lykke.Service.KucoinAdapter.Services.RestApi.Models
{
    public class GetBalanceResponse
    {
        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("datas")]
        public GetBalanceDataElement[] Datas { get; set; }

        [JsonProperty("currPageNo")]
        public long PageNumber { get; set; }

        [JsonProperty("limit")]
        public decimal PageSize { get; set; }

        [JsonProperty("pageNos")]
        public long TotalPages { get; set; }
    }
}
