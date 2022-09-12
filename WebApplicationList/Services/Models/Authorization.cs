using Microsoft.AspNetCore.Identity;
using WebApplicationList.ApplicationDataBase;
using WebApplicationList.IdentityApplication.ViewModels;
using WebApplicationList.Models;
using WebApplicationList.Models.MainSiteModels.ProfileModels;
using WebApplicationList.Services;


namespace WebApplicationList.Services.Models
{
    public class Authorization : IAuthorization
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private ApplicationDb _applicationDB;

        public Authorization(UserManager<User> userManager, SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager, ApplicationDb applicationDB)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _applicationDB = applicationDB;
        }
        async public Task<ViewAuthorizationModel> RegisterAsync(RegisterModel registerModel)
        {
            ViewAuthorizationModel model = new ViewAuthorizationModel();

            User user = new User
            {
                UserName = registerModel.Login,
                Email = registerModel.Email,
            };


            var result = await _userManager.CreateAsync(user, registerModel.Password);

            if (result.Succeeded)
            {
                await _roleManager.CreateAsync(new IdentityRole("user"));
                await _userManager.AddToRoleAsync(user, "user");
                await _signInManager.SignInAsync(user, registerModel.RememberMe);
                await _applicationDB.profileUserInfo!.AddAsync(new ProfileUserInfo { UserId = user.Id });
                await _applicationDB!.SaveChangesAsync();
                model.IsSuccess();
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    model.Errors!.Add(error.Description);
                }
                model.NotSuccess();
            }
            return model;
        }
        async public Task<ViewAuthorizationModel> LoginAsync(LoginModel loginModel)
        {
            ViewAuthorizationModel modelAuthorize = new();

            var result = await _signInManager.PasswordSignInAsync(loginModel.Login, loginModel.Password, loginModel.RememberMe, false);

            if (result.Succeeded)
            {
                modelAuthorize.IsSuccess();
            }
            else
            {
                modelAuthorize.NotSuccess();
                modelAuthorize.Errors!.Add("Неправильный логин или пароль");
            }

            return modelAuthorize;
        }


        async public Task<string> GetUserAvatar(string username)
        {
            if (!string.IsNullOrWhiteSpace(username))
                return (await _userManager.FindByNameAsync(username)).LinkAvatar!;
            else
                return string.Empty;
        }

        async public Task<bool> Logout()
        {
            await _signInManager.SignOutAsync();
            return true;
        }

        public async Task<bool> CheckCookie(string name)
        {
            var user = await _userManager.FindByNameAsync(name);

            if (user == null)
            {
                await _signInManager.SignOutAsync();
                return false;
            }

            //if (await _userManager.IsInRoleAsync(user, "banned"))
            //{
            //    await _signInManager.SignOutAsync();
            //    return false;
            //}

            return true;
        }
    }
}
