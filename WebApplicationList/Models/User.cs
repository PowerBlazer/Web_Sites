using Microsoft.AspNetCore.Identity;
using WebApplicationList.Models.MainSiteModels.ProjectModels;

namespace WebApplicationList.Models
{
    public class User : IdentityUser
    {
        public User() => LinkAvatar = "UserIcons/defaultAvatar.jpg";
        public string? LinkAvatar { get; set; }
        public DateTime DateRegistraition { get; set; } = DateTime.Now;


        public List<ProjectComment> projectComments { get; set; } = new List<ProjectComment>();
        public List<ProjectLike> projectLikes { get; set; } = new List<ProjectLike>();

    }
}
