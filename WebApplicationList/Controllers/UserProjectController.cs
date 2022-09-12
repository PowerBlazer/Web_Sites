using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using WebApplicationList.Models.MainSiteModels.ViewModels;
using WebApplicationList.Services;

namespace WebApplicationList.Controllers
{
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
        async public Task<IActionResult> Index(string Site,string page)
        {
            string result =  await _projectSetting.GetHtmlProject(Site,page);

            if (string.IsNullOrEmpty(result))
                return StatusCode(404);

            return View((object)result);      
        }
        [HttpPost]
        async public Task<IActionResult> GetExplorer()
        {
            var user = await _profileUser.GetUserAsync();

            if (user == null)
            {
                throw new Exception("Пользователь не найден");
            }

            var file = Request.Form.Files[0];
           
            if (file is null||file.ContentType!= "application/x-zip-compressed"||file.Length>52428800)
            {
                throw new Exception("Файл пустой или превышает допустимый размер");
            }

            ////Получение распакованных файлов
            var result = await _projectSetting.GetExplorer(file.OpenReadStream(), user.UserName);

            if (result == null)
            {
                throw new ArgumentNullException("Ошибка на сервере пробуйте позже");
            }

            return PartialView("~/Views/UserProject/Partials/Explorer.cshtml",result);
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
            return await _projectSetting.GetValidationProjectName(projectName);
        }
        [HttpPost]
        public async Task<IActionResult> GetSettingsProject()
        {
           var user = await _profileUser.GetUserAsync();


           var result = _projectSetting.GetPagesProject(user.UserName);
          
           
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
                ProjectSettingsViewModel? projectSettings = JsonSerializer.Deserialize<ProjectSettingsViewModel>(jsonProjectSettingVM);

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
        
    
        private async Task<bool> CheckValidationPath(string path)
        {
            var user = await _profileUser.GetUserAsync();

            if (!path.Contains(user.UserName))
            {
                return false;
            }

            return true;
        }
        private bool CheckValidationPath(string path,string userName)
        {
            if (!path.Contains(userName))
            {
                return false;
            }
            return true;
        }

        [HttpGet]
        public IActionResult Test()
        {
            return View();
        }

        [HttpPost]
        public  JsonResult InputFile()
        {
            
            var result = Request.Form.Files[0].OpenReadStream();
            
            using (FileStream fileStream = new FileStream(@"C:\Users\power\Desktop\ТССА.zip", FileMode.Create))
            {
                result.CopyTo(fileStream);
            }
            
            return Json("123");
        }

        //[HttpGet]
        //public IActionResult Load([FromServices]ApplicationDataBase.ApplicationDb application)
        //{
        //    application.userProjects!.Add(new Models.MainSiteModels.ProfileModels.UserProject
        //    {
        //        Name = "Relvise",
        //        UrlImage = "UserProjectsImage/Relvise.jpg",
        //        Description = "Finance and Consultancy Solution",
        //        Type = "Сайт визитка,HTML,JS,CSS,Финансы",
        //        User_Id = "3ce8cc61-f0ac-47ab-b318-e9d497c9a623",
        //        Url = "?Site=Relvise&page=Index",
        //    });
        //    application.SaveChanges();

        //    return base.Content("");
        //}
        

    }
}
