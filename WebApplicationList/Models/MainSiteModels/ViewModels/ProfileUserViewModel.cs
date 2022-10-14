using WebApplicationList.Models.MainSiteModels.ProfileModels;

namespace WebApplicationList.Models.MainSiteModels.ViewModels
{
    public class ProfileUserViewModel
    {    
        public ProfileUserInfo? userInfo { get; set; }
        public int countProjects { get; set; }
        public int countLikes { get; set; }
        public int countViews { get; set; }
        public int countSubscriber { get; set; }
        public IEnumerable<LinksProfile>? linksProfile { get; set; }
        public List<LinkType>? linkTypes { get; set; }
    }
}
