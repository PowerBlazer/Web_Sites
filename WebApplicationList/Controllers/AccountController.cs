using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json;
using WebApplicationList.IdentityApplication.ViewModels;
using WebApplicationList.Services;

namespace WebApplicationList.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthorization _authorization;
        private readonly IProfileUser _profileUser;
        public AccountController(IAuthorization authorization,IProfileUser profileUser)
        {
            _authorization = authorization;
            _profileUser = profileUser;
        }


        [HttpPost]
        public async Task<IActionResult> Register(string registerModelJs)
        {
            if (!string.IsNullOrEmpty(registerModelJs))
            {
                try
                {
                    var registerModel = System.Text.Json.JsonSerializer.Deserialize<RegisterModel>(registerModelJs);

                    var authorizeModel = await _authorization.RegisterAsync(registerModel!);

                    return Ok(authorizeModel);
                }
                catch
                {
                    return StatusCode(500);
                }
            }
            else
            {
                return StatusCode(404);
            }
        }
        

        [HttpPost]
        public async Task<IActionResult> Login(string loginModelJs)
        {
            if (!string.IsNullOrEmpty(loginModelJs))
            {
                try
                {
                    var loginModel = System.Text.Json.JsonSerializer.Deserialize<LoginModel>(loginModelJs);

                    var authorizeModel = await _authorization.LoginAsync(loginModel!);

                    return Ok(authorizeModel);
                }
                catch
                {
                    return StatusCode(500);
                }
            }
            else
            {
                return StatusCode(404);
            }
        }
       
        [HttpPost]
        async public Task<bool> Logout() => await _authorization.Logout();

        [HttpPost]
        public async Task<bool> CheckCookie()
        {
            var userName = _profileUser.GetUserName();
            if (!string.IsNullOrEmpty(userName))
            {
                if (await _authorization.CheckCookie(userName))
                    return true;
            }
            return false;
        }


        [HttpPost]
        [Authorize(Roles = "user")]
        public async Task<ViewAuthorizationModel> ChangePassword(string jsonPasswordModel)
        {
            if (string.IsNullOrEmpty(jsonPasswordModel))
            {
                throw new Exception("Ошибка на сервере");
            }

            try
            {
                var passwordModel = JsonConvert.DeserializeObject<PasswordViewModel>(jsonPasswordModel);

                var user = await _profileUser.GetUserAsync();

                if (passwordModel is null || user is null)
                {
                    throw new Exception("Ошибка на сервере");
                }

                return await _authorization.ChangePassword(passwordModel,user);
            }
            catch
            {
                throw new Exception("Ошибка на сервере");
            }
        }
    }
}
