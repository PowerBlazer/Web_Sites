
using WebApplicationList.IdentityApplication.ViewModels;
using WebApplicationList.Models.Enitity;

namespace WebApplicationList.Services
{
    public interface IAuthorization
    {
        Task<ViewAuthorizationModel> RegisterAsync(RegisterModel registerModel);
        Task<ViewAuthorizationModel> LoginAsync(LoginModel loginModel);
        Task<bool> CheckCookie(string name); 
        Task<bool> Logout();
        Task<ViewAuthorizationModel> ChangePassword(PasswordViewModel passwordViewModel,User? user);

    }
}
