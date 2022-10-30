using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json;
using WebApplicationList.Models.Enitity;
using WebApplicationList.Models.ViewModels;
using WebApplicationList.Services;

namespace WebApplicationList.Controllers
{
    [Authorize(Roles = "user")]
    public class UserProjectController : Controller
    {
        private readonly IProjectSetting _projectSetting;
        private readonly IProfileUser _profileUser;

        public UserProjectController(IProjectSetting projectSetting, IProfileUser profileUser)
        {
            _projectSetting = projectSetting;
            _profileUser = profileUser;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(string Site, string page)
        {
            string result = await _projectSetting.GetHtmlProject(Site, page);

            if (string.IsNullOrEmpty(result))
                return StatusCode(404);

            return View((object)result);
        }
        [HttpPost]
        public async Task<IActionResult> AddComment(string text,int projectId)
        {
            var user = await _profileUser.GetUserAsync();

            if(user is null)
            {
                throw new Exception("Пользователь не найден");
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                return BadRequest("Поле не может быть пустым");
            }

            return Ok(await _projectSetting.AddComment(projectId, user, text));
        }
        [HttpPost]
        public async Task<IActionResult> GetExplorer()
        {
            var user = await _profileUser.GetUserAsync();

            if (user == null)
            {
                throw new Exception("Пользователь не найден");
            }

            var file = Request.Form.Files[0];

            if (file is null || file.ContentType != "application/x-zip-compressed" || file.Length > 52428800)
            {
                throw new Exception("Файл пустой или превышает допустимый размер");
            }

            ////Получение распакованных файлов
            var result = await _projectSetting.GetExplorer(file.OpenReadStream(), user.UserName);

            if (result == null)
            {
                throw new ArgumentNullException("Ошибка на сервере пробуйте позже");
            }

            return PartialView("~/Views/UserProject/Partials/Explorer.cshtml", result);
        }
        [HttpGet]
        public IActionResult GetExplorerStart()
        {
            return PartialView("~/Views/UserProject/Partials/ExplorerStart.cshtml");
        }
        [HttpPost]
        public async Task<IActionResult> GetExplorerItems(string path)
        {
            if(string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(path);
            }

            if (!await CheckValidationPath(path))
            {
                throw new Exception("Доступ закрыт");
            }

            var result = await _projectSetting.GetExplorerItem(path);

            return PartialView("~/Views/UserProject/Partials/ExplorerItem.cshtml",result);
        }
        [HttpPost]
        public async Task<IActionResult> GetFileContent(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(path, "Пустое значение");
            }

            if (!await CheckValidationPath(path))
            {
                throw new Exception("Доступ закрыт");
            }

            var result = await _projectSetting.GetContentFileAsync(path);

            return PartialView("~/Views/UserProject/Partials/ModalFileRedactor.cshtml",result);
        }
        [HttpPost]
        public async Task<bool> ChangeContentFile(string content,string path)
        {
            if(string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            if (!await CheckValidationPath(path))
            {
                throw new Exception("Доступ закрыт");
            }

            return await _projectSetting.ChangeContentFile(new FileItem
            {
                Path = path,
                Content = content,
            });
        }
        [HttpPost]
        public async Task<bool> GetValidationNameProject(string projectName)
        {
            return await _projectSetting.GetValidationProjectName(projectName, null);
        }
        [HttpPost]
        public async Task<IActionResult> GetSettingsProject()
        {
           var user = await _profileUser.GetUserAsync();

           var result = _projectSetting.GetPagesProject(user.UserName,string.Empty);
          
           return PartialView("~/Views/UserProject/Partials/Settings.cshtml",result);
        }
        [HttpPost]
        public async Task<IActionResult> FormattingFile(string path,string projectName)
        {
            var user = await _profileUser.GetUserAsync();

            if (!await CheckValidationPath(path))
            {
                throw new Exception("Нет доступа");
            }

            var result = _projectSetting.FormattingFile(path, projectName, user.UserName);

           

            return PartialView("~/Views/UserProject/Partials/ModalFormattingResult.cshtml",result);
        }
        [HttpPost]
        public async Task<bool> SaveProject(string jsonProjectSettingVM)
        {
            if(string.IsNullOrWhiteSpace(jsonProjectSettingVM))
            {
                throw new ArgumentNullException(jsonProjectSettingVM);
            }

            try
            {
                ProjectSettingsViewModel? projectSettings = System.Text.Json.JsonSerializer.Deserialize<ProjectSettingsViewModel>(jsonProjectSettingVM);

                var user = await _profileUser.GetUserAsync();

                if(projectSettings is null|| user is null)
                {
                    throw new Exception("Null");
                }


                if (!await _projectSetting.SaveProject(projectSettings,user))
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return true;
        }
        [HttpPost]
        public async Task<bool> PutLike(int projectId)
        {
            var user = await _profileUser.GetUserAsync();

            if(user is null)
            {
                throw new Exception("Пользователь не найден");
            }

            return await _projectSetting.SelectLike(projectId, user);
        }
        [HttpPost]
        public async Task<bool> DeleteLike(int projectId)
        {
            var user = await _profileUser.GetUserAsync();

            if(user is null)
            {
                throw new Exception("Пользователь не найден");
            }

            return await _projectSetting.DeleteLike(projectId, user);
        }
        [HttpPost]
        public async Task<bool> DeleteProject(string projectName)
        {
            User user = await _profileUser.GetUserAsync();

            if(user is null)
            {
                return false;
            }

            return await _projectSetting.DeleteProject(projectName, user);
            
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProjectOptions()
        {
            User? user = await _profileUser.GetUserAsync(); 

            if(user is null)
            {
                return Unauthorized();
            }
            
            string projectOptionJson = Request.Form["projectOptions"].FirstOrDefault()!;

            if(projectOptionJson is null|| string.IsNullOrEmpty(projectOptionJson))
            {
                return StatusCode(404);
            }

            try
            {
                ProjectOptions projectOptions = JsonConvert.DeserializeObject<ProjectOptions>(projectOptionJson);

                if(!await _projectSetting.GetValidationProjectName(projectOptions.Name!,projectOptions.Id))
                {
                    return BadRequest("Такое имя уже существует");
                }

                if (string.IsNullOrWhiteSpace(projectOptions.Name))
                {
                    return BadRequest("Название не может быть пустым");
                }

                if (Request.Form.Files.Count > 0)
                {
                    projectOptions.Image = Request.Form.Files[0];

                    if(projectOptions.Image.Length> 15728640)
                    {
                        return BadRequest("Файл превышает допустимый размер");
                    }

                    if(!(projectOptions.Image.ContentType == "image/png" ||
                        projectOptions.Image.ContentType == "image/jpeg"))
                    {
                        return BadRequest("Файл имеет не допустимый формат");
                    }


                }

                if(await _projectSetting.UpdateProjectOptions(projectOptions, user))
                {
                    return Ok("Успешно сохранено");
                }
                else
                {
                    return BadRequest("При сохранение произошла ошибка , попробуйте позже");
                }

            }
            catch
            {
                return StatusCode(505);
            }
        }
        

        


        private async Task<bool> CheckValidationPath(string path)
        {
            var user = await _profileUser.GetUserAsync();

            if(!path.Contains("wwwroot"))
                path = EncodingText.CustomEncoding.EncodeDecrypt(path);

            if (!path.Contains(user.UserName))
            {
                return false;
            }

            return true;
        }
    }
}
