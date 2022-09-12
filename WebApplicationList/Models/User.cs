using Microsoft.AspNetCore.Identity;
using WebApplicationList.Models.MainSiteModels;

namespace WebApplicationList.Models
{
    public class User:IdentityUser
    {
        public User() => LinkAvatar = "UserIcons/defaultAvatar.jpg";

        public string? LinkAvatar { get; set; }

    }
}
