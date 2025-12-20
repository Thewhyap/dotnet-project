using Gauniv.WebServer.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gauniv.WebServer.Controllers.Client
{
    [Authorize(Roles = "CLIENT")]
    public class UsersController(ApplicationDbContext _context, UserManager<User> _userManager) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var local_user = await _userManager.GetUserAsync(User);
            if (local_user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get all users
            var local_allUsers = await _context.Users
                .OrderBy(u => u.UserName)
                .ToListAsync();

            // Filter to keep only users with CLIENT role (exclude ADMIN)
            var local_clientUsers = new List<User>();
            foreach (var user in local_allUsers)
            {
                var local_roles = await _userManager.GetRolesAsync(user);
                if (local_roles.Contains("CLIENT"))
                {
                    local_clientUsers.Add(user);
                }
            }

            return View("~/Views/Client/Users.cshtml", local_clientUsers);
        }
    }
}

