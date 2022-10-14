namespace WebApplicationList.Models.MainSiteModels.ProfileModels
{
    public class LinksProfile
    {
        public int Id { get; set; }
        public string? Link { get; set; }
        public User? User { get; set; }
        public LinkType? LinkType { get; set; }
    }
}
