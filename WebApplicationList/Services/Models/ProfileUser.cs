using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplicationList.ApplicationDataBase;
using WebApplicationList.Models;
using WebApplicationList.Models.MainSiteModels.ProfileModels;
using WebApplicationList.Models.MainSiteModels.ViewModels;
using WebApplicationList.Services;

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

        private string GetUserName() => _signInManager.Context.User.Identity!.Name!;
        async public Task<User> GetUserAsync()
        {
            return await _userManager.FindByNameAsync(GetUserName());
        }
        async public Task<ProfileUserInfo> GetUserInfoAsync(string? id)
        {
            return await _applicationDB.profileUserInfo!.Where(p => p.UserId == id).FirstOrDefaultAsync();
        }
        async public Task<bool> ChangeAvatarAsync(string byteImage)
        {
            var userName = GetUserName();

            if (string.IsNullOrWhiteSpace(byteImage))
                return false;

            string JpgMagicValue = "/9j/4AAQSkZJRgA";
            string PngMagicValue = "iVBORw0KGgoAAA";

            var checkFormImage = new List<string> { byteImage };

            int valuecheck = checkFormImage.Where(p => p.Contains(JpgMagicValue) || p.Contains(PngMagicValue)).Count();

            if (valuecheck == 0)
                return false;

            string filePath = Path.Combine(_hostEnvironment.WebRootPath, "UserIcons");
            var files = Directory.GetFiles(filePath).ToList();
            var result = files.Where(p => p.Contains($"{userName}")).FirstOrDefault();

            if (!string.IsNullOrEmpty(result))
            {
                File.Delete(result);
            }


            var bytefile = Convert.FromBase64String(byteImage);

            if (bytefile.Length > 20971520)
                return false;

            string PathAvatar = $"wwwroot/UserIcons/{userName}.jpg";

            using (FileStream fileStream = new FileStream(PathAvatar, FileMode.Create, FileAccess.ReadWrite))
            {
                await fileStream.WriteAsync(bytefile, 0, bytefile.Length);
            }

            var user = await _applicationDB.Users.Where(p => p.UserName == userName).FirstOrDefaultAsync();
            _applicationDB.Attach(user!);
            user!.LinkAvatar = $"UserIcons/{userName}.jpg";
            await _applicationDB.SaveChangesAsync();

            return true;
        }
        public async Task<bool> ChangeDescriptionAsync(string Description)
        {
            var user = await GetUserAsync();
            if (user == null)
                return false;

            var userInfo = await _applicationDB.profileUserInfo!.Where(p => p.UserId == user!.Id).FirstOrDefaultAsync();
            if (userInfo == null)
                return false;

            _applicationDB.Attach(userInfo!);

            userInfo.Description = Description;

            await _applicationDB.SaveChangesAsync();

            return true;
        }
        async public Task<int> GetNumberSubdcribes(string id)
        {
            return await _applicationDB.subscribesProfile!.Where(p => p.UserId == id).CountAsync();
        }
        async public Task<int> GetNumberLikes(string id)
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
                userInfo = await GetUserInfoAsync(user.Id),
                NumberSubscriber = await GetNumberSubdcribes(user.Id),
                NumberLikes = await GetNumberLikes(user.Id),
                userProjects = await GetProjectsUser(user.Id),
                favoritesProjects = await GetFavoritesProject(user.Id),
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
    }
}
