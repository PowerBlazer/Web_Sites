using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplicationList.Models;
using WebApplicationList.Services;

namespace WebApplicationList.Controllers.MainController
{
    [Authorize(Roles = "user")]
    public class ProfileController : Controller
    {
        private readonly IProfileUser _profileUser;
        private readonly IProjectSetting _projectSetting;
        public ProfileController(IProfileUser profileUser, IProjectSetting projectSetting)
        {
            _profileUser = profileUser;
            _projectSetting = projectSetting;
        }

        [HttpGet]
        [Route("Profile")]
        async public Task<IActionResult> ProfilePage(string page)
        {
            var user = await _profileUser.GetUserAsync();

            var userInfo = await _profileUser.GetUserViewModelAsync(user);

            if (userInfo == null)
                return Redirect("~/Main");

            ViewBag.Page = page;

            return View("~/Views/Main/Profile.cshtml", userInfo);
        }

        [HttpPost]
        async public Task<bool> ChangeAvatar(string? byteImage)
        {
            return await _profileUser.ChangeAvatarAsync(byteImage!);
        }
        [HttpPost]
        async public Task<bool> ChangeDescription(string description)
        {
            return await _profileUser.ChangeDescriptionAsync(description);
        }

        async public Task<string> GetUserName() => (await _profileUser.GetUserAsync()).UserName;
       
    }
}
