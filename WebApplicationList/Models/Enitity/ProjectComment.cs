using System.ComponentModel.DataAnnotations;

namespace WebApplicationList.Models.Enitity
{
    public class ProjectComment
    {
        [Key]
        public int Id { get; set; }
        public User? user { get; set; }
        public UserProject? project { get; set; }
        public string? Text { get; set; }
        public DateTime date { get; set; } = DateTime.Now;
    }
}
