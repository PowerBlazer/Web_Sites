using System.ComponentModel.DataAnnotations;

namespace WebApplicationList.Models.MainSiteModels.ProjectModels
{
    public class ProjectView
    {
        [Key]
        public int Id { get; set; }
        public User? user { get; set; }
        public UserProject? project { get; set; }
    }
}
