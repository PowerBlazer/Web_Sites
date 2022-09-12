using System.ComponentModel.DataAnnotations;

namespace WebApplicationList.Models.MainSiteModels.ProfileModels
{
    public class ProfileUserInfo
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? Surname { get; set; }
        public string? Description { get; set; }
        public string? Profession { get; set; }
        public int Year { get; set; }
        public DateTime DateRegistraition { get; set; } = DateTime.Now;
        
    }
}
