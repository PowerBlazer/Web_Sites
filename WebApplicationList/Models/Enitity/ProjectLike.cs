using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationList.Models.Enitity
{
    [Table("ProjectLikes")]
    public class ProjectLike
    {
        [Key]
        public int Id { get; set; }
        public User? user { get; set; }
        public Project? project { get; set; }
    }
}
