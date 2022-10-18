using WebApplicationList.Models.Enitity;
using WebApplicationList.Models.MainSiteModels.ProfileModels;

namespace WebApplicationList.Models.MainSiteModels.ViewModels
{
    public class ProfileUserViewModel
    {    
        //public ProfileUserInfo? userInfo { get; set; }
        public string? UserName { get; set; }
        public string? LinkAvatar { get; set; }
        public string? Email { get; set; }
        public string? Surname { get; set; }
        public int? Year { get; set; }
        public string? Profession { get; set; }
        public string? HeaderDescriprion { get; set; }
        public string? Description { get; set; }
        public DateTime DateRegistration { get; set; }
        public int countProjects { get; set; }
        public int countLikes { get; set; }
        public int countViews { get; set; }
        public int countSubscriber { get; set; }
        public int countSubscriptions { get; set; }
        public bool signed { get; set; }
        public IEnumerable<LinksProfile>? linksProfile { get; set; }
        public List<LinkType>? linkTypes { get; set; }
    }
}
