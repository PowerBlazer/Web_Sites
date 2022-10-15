using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using WebApplicationList.Models.Enitity;
using WebApplicationList.Models.MainSiteModels.ProfileModels;
using WebApplicationList.Models.MainSiteModels.ProjectModels;

namespace WebApplicationList.ApplicationDataBase
{
    public class ApplicationDb : IdentityDbContext<User>
    {
        public DbSet<UserProject>? userProjects { get; set; }
        public DbSet<ProfileUserInfo>? profileUserInfo { get; set; }        
        public DbSet<LinkType>? linksType { get; set; }
        public DbSet<LinksProfile>? linksProfile { get; set; }
        public DbSet<ProjectComment>? projectComments { get; set; }
        public DbSet<ProjectLike>? projectLikes { get; set; }
        public DbSet<ProjectView>? projectViews { get; set; }
        public DbSet<SubscribeUser>? subscribeUsers { get; set; }

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
                    Id = 1,
                    Name = "vkontakte",
                },
                new LinkType
                {
                    Id = 2,
                    Name = "facebook"
                },
                new LinkType
                {
                    Id = 3,
                    Name = "telegram"
                },
                new LinkType
                {
                    Id = 4,
                    Name = "twitter"
                },
                new LinkType
                {
                    Id = 5,
                    Name = "instagram",
                }
            );
            builder
             .Entity<User>()
             .HasOne(u => u.profileUserInfo)
             .WithOne(p => p.user)
             .HasForeignKey<ProfileUserInfo>(p => p.User_key);

            builder.Entity<SubscribeUser>()
                .HasOne(p => p.user)
                .WithMany(p => p.usersProfile);

            builder.Entity<SubscribeUser>()
                .HasOne(p=>p.subscribe)
                .WithMany(p=>p.subscribes);
                
                

           



        }

    }
}
