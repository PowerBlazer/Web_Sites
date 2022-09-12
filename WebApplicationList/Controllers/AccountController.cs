using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebApplicationList.IdentityApplication.ViewModels;
using WebApplicationList.Services;

namespace WebApplicationList.Controllers
{
    public class AccountController : Controller
    {
        IAuthorization _authorization;
        public AccountController(IAuthorization authorization)
        {
            _authorization = authorization;
        }


        [HttpPost]
        async public Task<IActionResult> Register(string registerModelJs)
        {
            if (!string.IsNullOrEmpty(registerModelJs))
            {
                try
                {
                    var registerModel = JsonSerializer.Deserialize<RegisterModel>(registerModelJs);

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
        async public Task<IActionResult> Login(string loginModelJs)
        {
            if (!string.IsNullOrEmpty(loginModelJs))
            {
                try
                {
                    var loginModel = JsonSerializer.Deserialize<LoginModel>(loginModelJs);

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
        async public Task<string> GetAvatar(string username)
        {
            return await _authorization.GetUserAvatar(username);
        }

        [HttpPost]
        async public Task<bool> Logout() => await _authorization.Logout();

        [HttpPost]
        public async Task<bool> CheckCookie(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                if (await _authorization.CheckCookie(name))
                    return true;
            }
            return false;
        }




    }
}
