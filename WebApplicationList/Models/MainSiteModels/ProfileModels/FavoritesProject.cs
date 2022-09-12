using System.ComponentModel.DataAnnotations;

namespace WebApplicationList.Models.MainSiteModels.ProfileModels
{
    public class FavoritesProject
    {
        [Key]
        public int Id { get; set; }
        public string? User_Id { get; set; }
        public string? Project_Id { get; set; }
    }
}
