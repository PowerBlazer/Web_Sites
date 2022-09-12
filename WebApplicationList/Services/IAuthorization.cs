
using WebApplicationList.IdentityApplication.ViewModels;

namespace WebApplicationList.Services
{
    public interface IAuthorization
    {
        public Task<ViewAuthorizationModel> RegisterAsync(RegisterModel registerModel);

        public Task<ViewAuthorizationModel> LoginAsync(LoginModel loginModel);

        public Task<bool> CheckCookie(string name); 
        public Task<bool> Logout();

        public Task<string> GetUserAvatar(string username);
    }
}
