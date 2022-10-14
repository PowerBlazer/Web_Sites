using WebApplicationList.Models.MainSiteModels.ProfileModels;
using WebApplicationList.Models.MainSiteModels.ProjectModels;

namespace WebApplicationList.Models.MainSiteModels.ViewModels
{
    public class ProjectViewModel
    {    
        public ProfileUserInfo? userInfo { get; set; }
        public UserProject? project { get; set; }
        public List<ProjectComment>? projectComments { get; set; }
        public int likes { get; set; }
        public int views { get; set; }
        public bool liked { get; set; }
        public bool signed { get; set; }
    }
}
