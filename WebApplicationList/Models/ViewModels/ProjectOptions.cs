using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;

namespace WebApplicationList.Models.ViewModels
{
    public class ProjectOptions
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        [JsonIgnore]
        public IFormFile? Image { get; set; }
        [JsonIgnore]
        public string? Url { get; set; }
        [JsonIgnore]
        public string? ImagePath { get; set; }
        public string? SelectedPage { get; set; }
        public string? Description { get; set; }
        public string? Types { get; set; }
        [JsonIgnore]
        public IEnumerable<FileItem>? pagesProject { get; set; }
    }
}
