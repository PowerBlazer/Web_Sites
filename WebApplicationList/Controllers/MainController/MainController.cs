﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApplicationList.Models.MainSiteModels.ViewModels;
using WebApplicationList.Services;

namespace WebApplicationList.Controllers.MainController
{
    public class MainController : Controller
    {
        private readonly ISearch _searchService;
        public MainController(ISearch searchService)
        {
            _searchService = searchService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Types = await _searchService.GetTypesProject();

            return View(await _searchService.GetProjects());
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

    }
}
