using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApplicationList.Models.MainSiteModels.ViewModels;
using WebApplicationList.Services;

namespace WebApplicationList.Controllers.MainController
{
    public class MainController : Controller
    {
        private readonly ISearch _searchService;
        private readonly IProfileUser _profileUser;
             
        public MainController(ISearch searchService,IProfileUser profileUser)
        {
            _searchService = searchService;  
            _profileUser = profileUser;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Types = await _searchService.GetTypesProject();

            return View("~/Views/Main/Index.cshtml",await _searchService.GetProjects());
        }
        [HttpPost]
        public async Task<IActionResult> SearchProjects(string searchOptionsJson)
        {
            if(string.IsNullOrEmpty(searchOptionsJson))
            {
                return base.Content("Пустое значение");
            }

            try
            {
                var options = JsonConvert.DeserializeObject<SearchOptions>(searchOptionsJson);

                if(options is null)
                {
                    return StatusCode(404);
                }

                var result = await _searchService.GetProjectsApplyFilters(options);

                return PartialView("~/Views/Main/Partials/SearchProjectView.cshtml",result); 
            }
            catch
            {
                return StatusCode(500);
            }
        }
        [HttpPost]
        public async Task<IActionResult> SearchUsers(string searchOptionsJson)
        {
            if (string.IsNullOrEmpty(searchOptionsJson))
            {
                return base.Content("Пустое значение");
            }

            try
            {
                var options = JsonConvert.DeserializeObject<SearchOptions>(searchOptionsJson);

                if (options is null)
                {
                    return StatusCode(404);
                }

                var result = await _searchService.GetUsersApplyFilters(options);

                return PartialView("~/Views/Main/Partials/SearchUsersView.cshtml",result);
            }
            catch
            {
                return StatusCode(500);
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetProjectInfo(string projectName, [FromServices] IProjectSetting projectSetting)
        {
            if(string.IsNullOrEmpty(projectName))
            {
                throw new Exception(projectName);
            }

            var user = await _profileUser.GetUserAsync();

            if(user is not null)
            {
                await projectSetting.SetViewProject(projectName, user);
            }

            var result = await _searchService.GetProjectPresentation(projectName);

            return PartialView("~/Views/Main/Partials/ProjectInfo.cshtml", result);
        }
        [HttpPost]
        public async Task<IActionResult> GetCommentsProject(int count,int projectId)
        {
         
            return PartialView("~/Views/Main/Partials/CommentsView.cshtml",await _searchService.GetComments(count,projectId));
        }
                  
    }
}
