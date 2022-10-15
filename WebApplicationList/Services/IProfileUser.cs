using Microsoft.AspNetCore.Identity;
using WebApplicationList.Models.Enitity;
using WebApplicationList.Models.MainSiteModels.ProfileModels;
using WebApplicationList.Models.MainSiteModels.ViewModels;

namespace WebApplicationList.Services
{
    public interface IProfileUser
    {
        string GetUserName();
        Task<User> GetUserAsync();
        Task<User> GetUserForLogin(string login);
        Task<ProfileUserInfo> GetUserProfileInfoAsync(string id);
        Task<ProfileUserViewModel> GetProfileUserViewModelAsync(User user);
        Task<bool> ChangeAvatarAsync(IFormFile imageFile);
        Task<bool> ChangeUserInfo(ProfileUserInfoViewModel profileUserInfo);
        Task<IEnumerable<LinksProfile>> GetLinksUser(string userId);
        Task<bool> BindLinkInUser(string url, int id);      
        Task<IEnumerable<ProjectViewModel>> GetProjectsUser(string id);
        Task<bool> SetSubsccribe(User user, User subscribeUser);
        Task<bool> DeleteSubscribe(User user, User subscribe);
    }
}
