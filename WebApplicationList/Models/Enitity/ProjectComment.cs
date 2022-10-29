using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationList.Models.Enitity
{
    [Table("ProjectComments")]
    public class ProjectComment
    {
        [Key]
        public int Id { get; set; }
        public User? user { get; set; }
        public Project? project { get; set; }
        public string? Text { get; set; }
        public DateTime date { get; set; } = DateTime.Now;
    }
}
