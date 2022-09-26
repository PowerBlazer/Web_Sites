using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using WebApplicationList.Models;
using WebApplicationList.Models.MainSiteModels.ProfileModels;

namespace WebApplicationList.ApplicationDataBase
{
    public class ApplicationDb : IdentityDbContext<User>
    {
        public DbSet<UserProject>? userProjects { get; set; }
        public DbSet<ProfileUserInfo>? profileUserInfo { get; set; }
        public DbSet<SubscribesProfile>? subscribesProfile { get; set; }
        public DbSet<LikesProfile>? likesProfiles { get; set; }
        public DbSet<FavoritesProject>? favoritesProjects { get; set; }
        public DbSet<LinkType>? linksType { get; set; }
        public DbSet<LinksProfile>? linksProfile { get; set; }

        public ApplicationDb(DbContextOptions<ApplicationDb> options) : base(options)
        {
            Database.EnsureCreated();
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<LinkType>().HasData(
                new LinkType
                {
                    Id=1,
                    Name="vkontakte",
                },
                new LinkType
                {
                    Id=2,
                    Name= "facebook"
                },
                new LinkType
                {
                    Id=3,
                    Name= "telegram"
                },
                new LinkType
                {
                    Id=4,
                    Name= "twitter"
                },
                new LinkType
                {
                    Id=5,
                    Name= "instagram",
                }
            );
            
        }

    }
}
