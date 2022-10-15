using System.ComponentModel.DataAnnotations;
using WebApplicationList.Models.Enitity;

namespace WebApplicationList.Models.MainSiteModels.ProjectModels
{
    public class ProjectLike
    {
        [Key]
        public int Id { get; set; }
        public User? user { get; set; }
        public UserProject? project { get; set; }
    }
}
