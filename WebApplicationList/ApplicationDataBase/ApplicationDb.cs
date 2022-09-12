using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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

        public ApplicationDb(DbContextOptions<ApplicationDb> options) : base(options)
        {
            Database.EnsureCreated();
        }

        
    }
}
