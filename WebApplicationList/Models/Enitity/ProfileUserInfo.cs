using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationList.Models.Enitity
{
    [Table("UserInfo")]
    public class ProfileUserInfo
    {
        [Key]
        public int Id { get; set; }
        public string? Surname { get; set; }
        public string? HeaderDescription { get; set; }
        public string? Description { get; set; }
        public string? Profession { get; set; }
        public int? Year { get; set; }
        public string? User_key { get; set; }
        public User? user { get; set; }
    }
}
