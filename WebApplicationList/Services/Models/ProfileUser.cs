using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplicationList.ApplicationDataBase;
using WebApplicationList.Models;
using WebApplicationList.Models.MainSiteModels.ProfileModels;
using WebApplicationList.Models.MainSiteModels.ViewModels;

namespace WebApplicationList.Services.Models
{
    public class ProfileUser : IProfileUser
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDb _applicationDB;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProfileUser(SignInManager<User> signInManager, UserManager<User> userManager,
            ApplicationDb applicationDB, IWebHostEnvironment hostEnvironment)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _applicationDB = applicationDB;
            _hostEnvironment = hostEnvironment;
        }

        public string GetUserName() => _signInManager.Context.User.Identity!.Name!;
        public async Task<User> GetUserAsync()
        {
            return await _userManager.FindByNameAsync(GetUserName());
        }
        public async Task<ProfileUserInfo> GetUserProfileInfoAsync(string? id)
        {
            return await _applicationDB.profileUserInfo!.Where(p => p.UserId == id).FirstOrDefaultAsync();
        }
        public async Task<bool> ChangeAvatarAsync(IFormFile image)
        {
            var userName = GetUserName();

            if (!(image.ContentType == "image/png" || image.ContentType == "image/jpeg") ||
                image.Length > 20971520)
            {
                return false;
            }

            string filePath = Path.Combine(_hostEnvironment.WebRootPath, "UserIcons");
            var files = Directory.GetFiles(filePath).ToList();

            var result = files.Where(p => p.Contains($"{userName}")).FirstOrDefault();

            if (!string.IsNullOrEmpty(result))
            {
                File.Delete(result);
            }
    
            string PathAvatar = $"wwwroot/UserIcons/{userName}.jpg";

            using (FileStream fileStream = new FileStream(PathAvatar, FileMode.Create, FileAccess.ReadWrite))
            {
                await image.OpenReadStream().CopyToAsync(fileStream);
            }

            var user = await _applicationDB.Users.Where(p => p.UserName == userName).FirstOrDefaultAsync();
            _applicationDB.Attach(user!);
            user!.LinkAvatar = $"UserIcons/{userName}.jpg";

            await _applicationDB.SaveChangesAsync();

            return true;
        }
        public async Task<int> GetNumberSubdcribes(string id)
        {
            return await _applicationDB.subscribesProfile!.Where(p => p.UserId == id).CountAsync();
        }
        public async Task<int> GetNumberLikes(string id)
        {
            return await _applicationDB.likesProfiles!.Where(p => p.UserId == id).CountAsync();
        }
        public async Task<string> GetDescription()
        {
            var user = await GetUserAsync();

            if (user == null)
                return string.Empty;

            var userInfo = await _applicationDB.profileUserInfo!.Where(p => p.UserId == user.Id).FirstOrDefaultAsync();

            if (userInfo == null)
                return string.Empty;

            return userInfo!.Description;
        }
        public async Task<IEnumerable<UserProject>> GetProjectsUser(string id)
        {
            return await _applicationDB.userProjects!.Where(p => p.User_Id == id).ToListAsync();
        }
        async public Task<ProfileUserViewModel> GetUserViewModelAsync(User user)
        {
            if (user == null)
                return null;

            ProfileUserViewModel profileUserView = new()
            {
                user = user,
                userInfo = await GetUserProfileInfoAsync(user.Id),
                NumberSubscriber = await GetNumberSubdcribes(user.Id),
                NumberLikes = await GetNumberLikes(user.Id),
                favoritesProjects = await GetFavoritesProject(user.Id),
                NumberProjects = (await GetProjectsUser(user.Id)).Count(),
                linkTypes = await _applicationDB.linksType!.ToListAsync(),
                linksProfile = await GetLinksUser(user.Id),
            };

            return profileUserView;
        }
        public async Task<User> GetUserForLogin(string login)
        {
            return await _applicationDB.Users.Where(p => p.UserName == login).FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<FavoritesProject>> GetFavoritesProject(string id)
        {
            return await _applicationDB.favoritesProjects!.Where(p => p.User_Id == id).ToListAsync();
        }
        public async Task<bool> ChangeUserInfo(ProfileUserInfoViewModel profileUserInfo)
        {
            if(profileUserInfo is null)
            {
                return false;
            }

            var user = await GetUserAsync();

            if(user is null)
            {
                return false;
            }

            var userInfo = await _applicationDB.profileUserInfo!.Where(p => p.UserId == user.Id).FirstOrDefaultAsync();

            if(userInfo is null) 
                return false;

            _applicationDB.Attach(userInfo);
            userInfo.HeaderDescription = profileUserInfo.HeaderDescription;
            userInfo.Description = profileUserInfo.Descrpition;
            userInfo.Year = profileUserInfo.Year;
            userInfo.Surname = profileUserInfo.Surname;
            userInfo.Profession = profileUserInfo.Profession;

            await _applicationDB.SaveChangesAsync();

            return true;
        }
        public async Task<bool> BindLinkInUser(string url,int id)
        {
            var linkType = await _applicationDB.linksType!.Where(p => p.Id == id).FirstOrDefaultAsync();

            if (linkType is null)
            {
                return false;
            }

            var user = await GetUserAsync();
            
            var linksUser = await _applicationDB.linksProfile!.Where(p=>p.LinkType!.Id==linkType.Id&&p.User_Id==user.Id).FirstOrDefaultAsync();

            if(linksUser is null)
            {
                if (string.IsNullOrEmpty(url))
                {
                    return true;
                }
            
                await _applicationDB.linksProfile!.AddRangeAsync(new LinksProfile
                {
                    LinkType = linkType,
                    User_Id = user.Id,
                    Link = url,
                });

                await _applicationDB.SaveChangesAsync();

                return true;
            }

            _applicationDB.Attach(linkType);
            _applicationDB.Attach(linksUser);

            if (string.IsNullOrEmpty(url))
            {
                 _applicationDB.linksProfile!.Remove(linksUser);
                await _applicationDB.SaveChangesAsync();
                return true;
            }

           

            linksUser.Link = url;

            await _applicationDB.SaveChangesAsync();

            return true;  
        }
        public async Task<IEnumerable<LinksProfile>> GetLinksUser(string userId)
        {
            return await _applicationDB.linksProfile!.Include(p => p.LinkType).Where(p => p.User_Id == userId).ToListAsync();
        }

       
    }
}
