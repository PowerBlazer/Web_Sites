using Microsoft.AspNetCore.Identity;

namespace WebApplicationList.Models
{
    public class User : IdentityUser
    {
        public User() => LinkAvatar = "UserIcons/defaultAvatar.jpg";

        public string? LinkAvatar { get; set; }
        public DateTime DateRegistraition { get; set; } = DateTime.Now;

    }
}
