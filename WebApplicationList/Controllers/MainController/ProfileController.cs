using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApplicationList.Models;
using WebApplicationList.Models.MainSiteModels.ViewModels;
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
        public async Task<IActionResult> ProfilePage(string page)
        {
            var user = await _profileUser.GetUserAsync();
       
            var userInfo = await _profileUser.GetUserViewModelAsync(user);

            if (userInfo == null)
                return Redirect("~/Main");

            ViewBag.Page = page;

            return View("~/Views/Main/Profile.cshtml", userInfo);
        }

        [HttpPost]
        public async Task<bool> ChangeAvatar()
        {
            var image = Request.Form.Files[0];

            if(image is null)
            {
                return false;
            }

            if(!await _profileUser.ChangeAvatarAsync(image))
            {
                return false;
            }

            return true;
        }
        [HttpPost]
        public async Task<bool> ChangeUserInfo(string jsonProfileUserInfo)
        {
            if(string.IsNullOrWhiteSpace(jsonProfileUserInfo))
            {
                return false;
            }

            try
            {
                var profileUserInfo = JsonConvert.DeserializeObject<ProfileUserInfoViewModel>(jsonProfileUserInfo);

                if(await _profileUser.ChangeUserInfo(profileUserInfo!))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        [HttpPost]
        public async Task<bool> BindLinkUser(string url,int id)
        {
            if(id==0)
            {
                return false;
            }

            if(await _profileUser.BindLinkInUser(url,id))
            {
                return true;
            }

            return false;
        }
        [HttpPost]
        public async Task<IActionResult> GetUserInfo()
        {
            var user = await _profileUser.GetUserAsync();

            if(user is null)
            {
                return StatusCode(404);
            }

            return Ok(new UserViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                LinkAvatar = user.LinkAvatar
            });
        }



        [HttpPost]
        public async Task<IActionResult> GetUserProjects(string login)
        {
            User? user;

            if (string.IsNullOrWhiteSpace(login))
            {
                user = await _profileUser.GetUserAsync();
            }
            else
            {

                user = await _profileUser.GetUserForLogin(login);
            }

            var result = await _profileUser.GetProjectsUser(user.Id);

            return PartialView("~/Views/Profile/Partials/ProjectsView.cshtml",result);
        }
       

        public async Task<string> GetUserName() => (await _profileUser.GetUserAsync()).UserName;
       
    }
}
