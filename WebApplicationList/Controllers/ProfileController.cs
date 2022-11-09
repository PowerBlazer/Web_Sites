using EncodingText;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApplicationList.Models.Enitity;
using WebApplicationList.Models.ViewModels;
using WebApplicationList.Services;


namespace WebApplicationList.Controllers
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
        [AllowAnonymous]
        public async Task<IActionResult> ProfilePage(string page,string userName)
        {
            User? user = default;
            if(!string.IsNullOrEmpty(userName))
            {
                user = await _profileUser.GetUserForLogin(userName);
                ViewBag.Privacy = false;
            }
            else
            {
                user = await _profileUser.GetUserAsync();
                ViewBag.Privacy = true;
            }

            if(user is null)
            {
                return StatusCode(404);
            }
             

            var userInfo = await _profileUser.GetProfileUserViewModelAsync(user);

            if (userInfo == null)
                return Redirect("~/Main");

            ViewBag.Page = page;

            return View("~/Views/Main/Profile.cshtml", userInfo);
        }
        [HttpPost]
        public async Task<bool> ChangeAvatar()
        {
            var image = Request.Form.Files[0];

            if (image is null)
            {
                return false;
            }

            if (!await _profileUser.ChangeAvatarAsync(image))
            {
                return false;
            }

            return true;
        }
        [HttpPost]
        public async Task<bool> ChangeUserInfo(string jsonProfileUserInfo)
        {
            if (string.IsNullOrWhiteSpace(jsonProfileUserInfo))
            {
                return false;
            }

            try
            {
                var profileUserInfo = JsonConvert.DeserializeObject<ProfileUserInfoViewModel>(jsonProfileUserInfo);

                if (await _profileUser.ChangeUserInfo(profileUserInfo!))
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
        public async Task<bool> BindLinkUser(string url, int id)
        {
            if (id == 0)
            {
                return false;
            }

            if (await _profileUser.BindLinkInUser(url, id))
            {
                return true;
            }

            return false;
        }
        [HttpPost]
        public async Task<IActionResult> GetUserInfo()
        {
            var user = await _profileUser.GetUserAsync();

            if (user is null)
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
        public async Task<bool?> SetSubscribe(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new Exception("Пустой значение");
            }

            var subscribeUser = await _profileUser.GetUserForLogin(userName);

            var user = await _profileUser.GetUserAsync();

            if (subscribeUser == user)
            {
                return null;
            }

            if (await _profileUser.SetSubsccribe(user, subscribeUser))
            {
                return true;
            }

            return false;
        }
        [HttpPost]
        public async Task<bool?> DeleteSubscribe(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new Exception("Пустой значение");
            }

            var subscribeUser = await _profileUser.GetUserForLogin(userName);

            var user = await _profileUser.GetUserAsync();

            if (subscribeUser == user)
            {
                return null;
            }

            if (await _profileUser.DeleteSubscribe(user, subscribeUser))
            {
                return true;
            }

            return false;
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> GetUserProjects(string login)
        {
            if (string.IsNullOrEmpty(login))
            {
                return StatusCode(404);
            }

            var user = await _profileUser.GetUserForLogin(login);
               
            if (user is null)
            {
                return StatusCode(404);
            }
                
            var result = await _profileUser.GetProjectsUser(user.Id);

            if (result.Count() == 0)
            {
                ViewBag.NullProjects = "У данного пользователя нету проектов";
            }

            return PartialView("~/Views/Main/Partials/SearchProjectView.cshtml", result);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> GetUserFavorites(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return StatusCode(404);
            }

            var user = await _profileUser.GetUserForLogin(userName);

            if(user is null)
            {
                return StatusCode(404);
            }

            var projects = await _profileUser.GetUserFavorites(user);

            if (projects.Count() == 0)
            {
                ViewBag.NullProjects = "Нету оцененных проектов";
            }

            return PartialView("~/Views/Main/Partials/SearchProjectView.cshtml", projects);


        }
        [HttpGet]
        public async Task<IActionResult> GetProjectElementsForSettings()
        {
            var user = await _profileUser.GetUserAsync();

            if(user is null)
            {
                return StatusCode(404);
            }

            var projects = await _profileUser.GetProjectsUser(user.Id);

            if(projects.Count() == 0)
            {
                ViewBag.NullProjects = "Нету добавленных проектов";
            }

            return PartialView("~/Views/Profile/Partials/ProjectsForSettings.cshtml",projects);
        }
        [HttpGet]
        public IActionResult GetDeleteProjectConfirmPanel()
        {
            return PartialView("~/Views/Profile/Partials/DeleteProjectConfirmPanel.cshtml");
        }
        [HttpGet]
        public async Task<IActionResult> GetFolderManagerProject(string projectName)
        {
            User user = await _profileUser.GetUserAsync();

            if(user is null)
            {
                return StatusCode(404);
            }

            string projectPath = _projectSetting.GetPathProject(projectName, user.UserName);

            if (string.IsNullOrEmpty(projectPath))
            {
                return StatusCode(404);
            }

            var folderItems = await _projectSetting.GetExplorerItem(CustomEncoding.EncodeDecrypt(projectPath));

            return PartialView("~/Views/Profile/Partials/FolderManagerProject.cshtml", folderItems);


        }
        [HttpGet]
        public async Task<IActionResult> GetFolderItems(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return StatusCode(404);
            }

            var folderItems = await _projectSetting.GetExplorerItem(path);

            return PartialView("~/Views/Profile/Partials/FolderManagerProject.cshtml", folderItems);
        }
        [HttpGet]
        public async Task<IActionResult> GetSettingPageProject(string projectName)
        {
            var user = await _profileUser.GetUserAsync();

            if(user is null)
            {
                return StatusCode(404);
            }

            ProjectOptions? projectOptions = await _projectSetting.GetProjectOptions(projectName, user);

            if(projectOptions is null)
            {
                return StatusCode(404);
            }

            return PartialView("~/Views/Profile/Partials/SettingsProject.cshtml",projectOptions);
        }

    }
}
