using Microsoft.AspNetCore.Identity;
using WebApplicationList.Models;
using WebApplicationList.Models.MainSiteModels.ProfileModels;
using WebApplicationList.Models.MainSiteModels.ViewModels;

namespace WebApplicationList.Services
{
    public interface IProfileUser
    {      
        Task<User> GetUserAsync();
        Task<User> GetUserForLogin(string login);
        Task<ProfileUserInfo> GetUserInfoAsync(string id);
        Task<ProfileUserViewModel> GetUserViewModelAsync(User user);
        Task<bool> ChangeAvatarAsync(string byteImage);
        Task<bool> ChangeDescriptionAsync(string Description);
        Task<int> GetNumberSubdcribes(string id);
        Task<int> GetNumberLikes(string id);
        Task<IEnumerable<UserProject>> GetProjectsUser(string id);
        Task<IEnumerable<FavoritesProject>> GetFavoritesProject(string id);
    }
}
