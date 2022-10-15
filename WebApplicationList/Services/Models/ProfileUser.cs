using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplicationList.ApplicationDataBase;
using WebApplicationList.Models.Enitity;
using WebApplicationList.Models.MainSiteModels.ProfileModels;
using WebApplicationList.Models.MainSiteModels.ViewModels;

namespace WebApplicationList.Services.Models
{
    public class ProfileUser : IProfileUser
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDb _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProfileUser(SignInManager<User> signInManager, UserManager<User> userManager,
            ApplicationDb applicationDB, IWebHostEnvironment hostEnvironment)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = applicationDB;
            _hostEnvironment = hostEnvironment;
        }

        public string GetUserName() => _signInManager.Context.User.Identity!.Name!;
        public async Task<User> GetUserAsync()
        {
            var userName = GetUserName();

            return await _context.Users.Where(p => p.UserName == userName).FirstOrDefaultAsync();
        }
        public async Task<ProfileUserInfo> GetUserProfileInfoAsync(string? id)
        {
            return await _context.profileUserInfo!.Where(p => p.user!.Id == id).FirstOrDefaultAsync();
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

            var user = await _context.Users.Where(p => p.UserName == userName).FirstOrDefaultAsync();
            _context.Attach(user!);
            user!.LinkAvatar = $"UserIcons/{userName}.jpg";

            await _context.SaveChangesAsync();

            return true;
        }   
        public async Task<IEnumerable<ProjectViewModel>> GetProjectsUser(string id)
        {
            var projects = await _context.userProjects!.OrderBy(p => p.Name)
                 .Where(p=>p.user!.Id == id)
                 .Include(p => p.user)
                 .Include(p => p.projectLikes)
                 .Include(p => p.projectViews)
                 .Select(p => new ProjectViewModel
                 {
                     userName = p.user!.UserName,
                     linkAvatar = p.user!.LinkAvatar,
                     projectName = p.Name,
                     projectUrl = p.Url,
                     projectUrlImage = p.UrlImage,
                     addedTime = p.AddedTime,
                     likes = p.projectLikes.Count(),
                     views = p.projectViews.Count(),
                 }).AsSplitQuery().AsNoTracking().Take(20).ToListAsync();

            return projects;
        }
        public async Task<ProfileUserViewModel> GetProfileUserViewModelAsync(User user)
        {
            if (user == null)
                return null;

            var linkTypes = await _context.linksType!.ToListAsync();

            var result = await _context.Users.Where(p => p.Id == user.Id)
                .Include(p => p.profileUserInfo)
                .Include(p => p.linksProfiles).ThenInclude(p=>p.LinkType)
                .Include(p=>p.projectViews)
                .Include(p=>p.projectLikes)
                .Include(p=>p.projects)
                .Include(p=>p.usersProfile)
                .Include(p=>p.subscribes)
                .Select(p => new ProfileUserViewModel
                {
                    UserName = p.UserName,
                    LinkAvatar = p.LinkAvatar,
                    Email = p.Email,
                    Year = p.profileUserInfo!.Year,
                    Profession = p.profileUserInfo!.Profession,
                    HeaderDescriprion = p.profileUserInfo.HeaderDescription,
                    Description = p.profileUserInfo.Description,
                    Surname = p.profileUserInfo.Surname,
                    DateRegistration = p.DateRegistraition,
                    linksProfile =  p.linksProfiles,
                    linkTypes =  linkTypes,
                    countLikes = p.projectLikes.Count(),
                    countSubscriber = p.subscribes.Count(),
                    countSubscriptions = p.usersProfile.Count(),
                    countProjects = p.projects.Count(),
                    countViews = p.projectViews.Count()

                }).AsNoTracking().FirstOrDefaultAsync();
          
            return result;
        }
        public async Task<User> GetUserForLogin(string login)
        {
            return await _context.Users.Where(p => p.UserName == login).FirstOrDefaultAsync();
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

            var userInfo = await _context.profileUserInfo!.Where(p => p.user!.Id == user.Id).FirstOrDefaultAsync();

            if(userInfo is null) 
                return false;

            _context.Attach(userInfo);

            userInfo.HeaderDescription = profileUserInfo.HeaderDescription;
            userInfo.Description = profileUserInfo.Descrpition;
            userInfo.Year = profileUserInfo.Year;
            userInfo.Surname = profileUserInfo.Surname;
            userInfo.Profession = profileUserInfo.Profession;

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> BindLinkInUser(string url,int id)
        {
            var linkType = await _context.linksType!.Where(p => p.Id == id).FirstOrDefaultAsync();

            if (linkType is null)
            {
                return false;
            }

            var user = await GetUserAsync();
            
            var linksUser = await _context.linksProfile!.Where(p=>p.LinkType!.Id==linkType.Id&&p.User!.Id==user.Id).FirstOrDefaultAsync();

            if(linksUser is null)
            {
                if (string.IsNullOrEmpty(url))
                {
                    return true;
                }
            
                await _context.linksProfile!.AddRangeAsync(new LinksProfile
                {
                    LinkType = linkType,
                    User = user,
                    Link = url,
                });

                await _context.SaveChangesAsync();

                return true;
            }

            _context.Attach(linkType);
            _context.Attach(linksUser);

            if (string.IsNullOrEmpty(url))
            {
                 _context.linksProfile!.Remove(linksUser);
                await _context.SaveChangesAsync();
                return true;
            }

           

            linksUser.Link = url;

            await _context.SaveChangesAsync();

            return true;  
        }
        public async Task<IEnumerable<LinksProfile>> GetLinksUser(string userId)
        {
            return await _context.linksProfile!.Include(p => p.LinkType).Where(p => p.User!.Id == userId).ToListAsync();
        }

        public async Task<bool> SetSubsccribe(User user,User subscribeUser)
        {
            if(user is null|| subscribeUser is null)
            {
                return false;
            }

            var checkSubscribe = await _context.subscribeUsers!
                .Where(p => p.user == user && p.subscribe == subscribeUser).FirstOrDefaultAsync();

            if(checkSubscribe is null)
            {
                await _context.subscribeUsers!.AddAsync(new SubscribeUser
                {
                    user = user,
                    subscribe = subscribeUser,
                });

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    return false;
                }
            }

            

            return true;
        }
        public async Task<bool> DeleteSubscribe(User user,User subscribe)
        {
            var subscriber = await _context.subscribeUsers!
                .Where(p => p.user == user && p.subscribe == subscribe).FirstOrDefaultAsync();

            if(subscriber is null)
            {
                return true;
            }

            _context.subscribeUsers!.Remove(subscriber);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                return false;
            }

            return true;
        }

       
    }
}
