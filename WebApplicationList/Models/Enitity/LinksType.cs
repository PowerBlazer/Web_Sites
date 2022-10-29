using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationList.Models.Enitity
{
    [Table("LinksType")]
    public class LinkType
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public List<LinksProfile> linksProfile = new List<LinksProfile>();
    }
}
