using Newtonsoft.Json;

namespace WebApplicationList.Models.MainSiteModels.ViewModels
{
    public class ProfileUserInfoViewModel
    {
        [JsonProperty("Surname")]
        public string? Surname { get; set; }
        [JsonProperty("Year")]
        public int? Year { get; set; }
        [JsonProperty("Profession")]
        public string? Profession { get; set; }
        [JsonProperty("HeaderDescription")]
        public string? HeaderDescription { get; set; }
        [JsonProperty("Description")]
        public string? Descrpition { get; set; }

    }
}
