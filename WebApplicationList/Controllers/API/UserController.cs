using Microsoft.AspNetCore.Mvc;
using WebApplicationList.ApplicationDataBase;

namespace WebApplicationList.Controllers.API
{
    [ApiController]
    [Route("api")]
    public class UserController:ControllerBase
    {
        private readonly ApplicationDb _contextDb;
        public UserController(ApplicationDb applicationDb)
        {
            _contextDb = applicationDb;
        }

        [HttpGet]
        [Route("GetUsers")]
        public IActionResult Index()
        {
            return Ok(_contextDb.userProjects!.ToList());
        }
    }
}
