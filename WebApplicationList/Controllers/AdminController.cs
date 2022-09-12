using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplicationList.Controllers
{
    [Authorize(Roles="admin")]
    public class AdminController:Controller
    {
        public AdminController()
        {

        }
        
        public IActionResult Index()
        {
            return View("~/Views/Main/Index.cshtml");
        }
    }
}
