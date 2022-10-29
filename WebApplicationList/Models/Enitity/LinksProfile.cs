using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationList.Models.Enitity
{
    [Table("LinksProfile")]
    public class LinksProfile
    {
        public int Id { get; set; }
        public string? Link { get; set; }
        public User? User { get; set; }
        public LinkType? LinkType { get; set; }
    }
}
