using Microsoft.AspNetCore.Identity;
using WebApplicationList.ApplicationDataBase;
using WebApplicationList.IdentityApplication.ViewModels;
using WebApplicationList.Models.Enitity;


namespace WebApplicationList.Services.Models
{
    public class Authorization : IAuthorization
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDb _applicationDB;

        public Authorization(UserManager<User> userManager, SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager, ApplicationDb applicationDB)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _applicationDB = applicationDB;
        }
        public async Task<ViewAuthorizationModel> RegisterAsync(RegisterModel registerModel)
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

                await _applicationDB.profileUserInfo!.AddAsync(new ProfileUserInfo { user = user });
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
        public async Task<ViewAuthorizationModel> LoginAsync(LoginModel loginModel)
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

        public async Task<ViewAuthorizationModel> ChangePassword(PasswordViewModel passwordViewModel,User? user)
        {
            if(user is null||string.IsNullOrEmpty(passwordViewModel.NewPassword)
                ||string.IsNullOrEmpty(passwordViewModel.OldPassword))
            {
                throw new ArgumentNullException(passwordViewModel.NewPassword);
            }

            ViewAuthorizationModel viewAuthorization = new();

            var result = await _userManager.ChangePasswordAsync(user, passwordViewModel.OldPassword, passwordViewModel.NewPassword);

            if (result.Succeeded)
            {
                viewAuthorization.IsSuccess();
            }
            else
            {
                viewAuthorization.NotSuccess();
                foreach(var item in result.Errors)
                {
                    viewAuthorization.Errors!.Add(item.Description);
                }
            }

            return viewAuthorization;
        }

        public Task<string> GetUserInfo()
        {
            throw new NotImplementedException();
        }
    }
}
