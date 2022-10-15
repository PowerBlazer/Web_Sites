using Microsoft.AspNetCore.Identity;
using WebApplicationList.Models.MainSiteModels.ProfileModels;
using WebApplicationList.Models.MainSiteModels.ProjectModels;

namespace WebApplicationList.Models.Enitity
{
    public class User : IdentityUser
    {
        public User() => LinkAvatar = "UserIcons/defaultAvatar.jpg";
        public string? LinkAvatar { get; set; }
        public DateTime DateRegistraition { get; set; } = DateTime.Now;

        public ProfileUserInfo? profileUserInfo { get; set; }
        public List<UserProject> projects { get; set; } = new List<UserProject>();
        public List<LinksProfile> linksProfiles { get; set; } = new List<LinksProfile>();
        public List<ProjectComment> projectComments { get; set; } = new List<ProjectComment>();
        public List<ProjectLike> projectLikes { get; set; } = new List<ProjectLike>();
        public List<ProjectView> projectViews { get; set; } = new List<ProjectView>();
        public List<SubscribeUser> subscribes { get; set; } = new List<SubscribeUser>();
        public List<SubscribeUser> usersProfile { get; set; } = new List<SubscribeUser>();
    }
}
