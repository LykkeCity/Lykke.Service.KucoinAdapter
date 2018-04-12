using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lykke.Service.KucoinAdapter.Services.RestApi.Models
{
    public sealed class DealOrders
    {
        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("firstPage")]
        public bool FirstPage { get; set; }

        [JsonProperty("lastPage")]
        public bool LastPage { get; set; }

        [JsonProperty("datas")]
        public IReadOnlyCollection<OrderDetailsDeal> Datas { get; set; }

        [JsonProperty("currPageNo")]
        public long CurrPageNo { get; set; }

        [JsonProperty("limit")]
        public long Limit { get; set; }

        [JsonProperty("pageNos")]
        public long PageNos { get; set; }
    }
}
