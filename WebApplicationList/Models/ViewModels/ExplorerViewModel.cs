namespace WebApplicationList.Models.ViewModels
{
    public class ExplorerViewModel
    {
        public List<FileItem>? Files { get; set; }
        public List<FileItem>? Folders { get; set; }
        public string? ProjectName { get; set; }
        public string? ReturnPath { get; set; }
        public string? CurrentPath { get; set; }
    }
    public class FileItem
    {
        public string? Path { get; set; }
        public string? Name { get; set; }
        public string? Size { get; set; }
        public string? Content { get; set; }
    }
   
  
}
