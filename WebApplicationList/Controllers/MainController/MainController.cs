using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationList.ApplicationDataBase;
using WebApplicationList.Models.MainSiteModels.ProfileModels;

namespace WebApplicationList.Controllers.MainController
{
    public class MainController : Controller
    {
        
        public MainController()
        {
            
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
