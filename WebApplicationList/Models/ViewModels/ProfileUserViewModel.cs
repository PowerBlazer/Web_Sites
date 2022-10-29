using WebApplicationList.Models.Enitity;
using WebApplicationList.Models.ViewModels;

namespace WebApplicationList.Models.ViewModels
{
    public class ProfileUserViewModel
    {    
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
        public List<int>? countLikes { get; set; } = new List<int>();
        public List<int>? countViews { get; set; } = new List<int>();
        public int countSubscriber { get; set; }
        public int countSubscriptions { get; set; }
        public bool signed { get; set; }

        public IEnumerable<LinksProfile>? linksProfile { get; set; }
        public List<LinkType>? linkTypes { get; set; }
    }
}
