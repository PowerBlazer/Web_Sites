namespace WebApplicationList.Models.MainSiteModels.ProfileModels
{
    public class LinkType
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public List<LinksProfile> linksProfile = new List<LinksProfile>();
    }
}
