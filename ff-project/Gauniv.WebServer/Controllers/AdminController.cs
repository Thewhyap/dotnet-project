using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gauniv.WebServer.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class AdminController :  Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }
        
        
        public IActionResult Games()
        {
            return View();
        }
        
        public IActionResult Categories()
        {
            return View();
        }
    }
    
}