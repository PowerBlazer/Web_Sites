using Newtonsoft.Json;

namespace WebApplicationList.Models.ViewModels
{
    public class SearchOptions
    {
        [JsonProperty("Text")]
        public string? Text { get; set; }
        [JsonProperty("Type")]
        public string? Type { get; set; }
        [JsonProperty("SortType")]
        public string? SortType { get; set; }
        [JsonProperty("PageIndex")]
        public int PageIndex { get; set; }
        
    }
}
