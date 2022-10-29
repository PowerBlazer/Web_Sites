using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationList.Models.Enitity
{
    [Table("Projects")]
    public class Project
    {
        public Project()
        {
            AddedTime = DateTime.Now;
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? UrlImage { get; set; }
        public string? Url { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public DateTime AddedTime { get; set; }

        public User? user { get; set; }
        public List<ProjectComment> projectComments { get; set; } = new List<ProjectComment>();
        public List<ProjectLike> projectLikes { get; set; } = new List<ProjectLike>();
        public List<ProjectView> projectViews { get; set; } = new List<ProjectView>();

    }
}
