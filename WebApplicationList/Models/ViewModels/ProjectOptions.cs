using Microsoft.Extensions.FileProviders;

namespace WebApplicationList.Models.ViewModels
{
    public class ProjectOptions
    {
        public string? Name { get; set; }
        public IFileInfo? Image { get; set; }
        public string? Url { get; set; }
        public string? ImagePath { get; set; }
        public string? SelectedPage { get; set; }
        public string? Description { get; set; }
        public string? Types { get; set; }
        public IEnumerable<FileItem>? pagesProject { get; set; }
    }
}
