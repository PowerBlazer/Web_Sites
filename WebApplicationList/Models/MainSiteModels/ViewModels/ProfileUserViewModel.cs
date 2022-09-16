using WebApplicationList.Models.MainSiteModels.ProfileModels;

namespace WebApplicationList.Models.MainSiteModels.ViewModels
{
    public class ProfileUserViewModel
    {
        public User? user { get; set; }
        public ProfileUserInfo? userInfo { get; set; }
        public int NumberSubscriber { get; set; }
        public int NumberLikes { get; set; }
        public int NumberProjects { get; set; }
        public IEnumerable<LinksProfile>? linksProfile { get; set; }
        public IEnumerable<LinkType>? linkTypes { get; set; }
        public IEnumerable<FavoritesProject>? favoritesProjects { get; set; }
    }
}
