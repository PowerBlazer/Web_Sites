using WebApplicationList.Models.Enitity;

namespace WebApplicationList.Models.ViewModels
{
    public class ProjectViewModel
    {    
        public string? userName { get; set; }
        public string? linkAvatar { get; set; }
        public string? profession { get; set; }
        public int projectId { get; set; }
        public string? projectName { get; set; }
        public string? projectUrl { get; set; }
        public string? projectUrlImage { get; set; }
        public string? projectDescription { get; set; }
        public string? projectTypes { get; set; }
        public DateTime addedTime { get; set; }
        public List<ProjectComment>? projectComments { get; set; }
        public int likes { get; set; }
        public int views { get; set; }
        public int commentsValue { get; set; }
        public bool liked { get; set; }
        public bool signed { get; set; }
    }
}
