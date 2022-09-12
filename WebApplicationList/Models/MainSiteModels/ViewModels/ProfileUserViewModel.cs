using WebApplicationList.Models.MainSiteModels.ProfileModels;

namespace WebApplicationList.Models.MainSiteModels.ViewModels
{
    public class ProfileUserViewModel
    {
        public User? user { get; set; }
        public ProfileUserInfo? userInfo { get; set; }
        public int NumberSubscriber { get; set; }
        public int NumberLikes { get; set; }
        public IEnumerable<UserProject>? userProjects { get; set; }
        public IEnumerable<FavoritesProject>? favoritesProjects { get; set; }
    }
}
