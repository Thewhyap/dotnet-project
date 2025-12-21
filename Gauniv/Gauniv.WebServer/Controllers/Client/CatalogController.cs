using Gauniv.WebServer.Data;
using Gauniv.WebServer.Dtos.Game;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gauniv.WebServer.Controllers.Client
{
    
    public class CatalogController(ApplicationDbContext _context, UserManager<User> _userManager): Controller
    {
        public async Task<IActionResult> Index()
        {
            var local_user = await _userManager.GetUserAsync(User);
            if (local_user == null)
            {
                return NotFound();
            }

            var userGames = await _context.Users
                .Where(gu => gu.Id == local_user.Id)
                .SelectMany(gu => gu.UserGames)
                .ToListAsync();
            
            return View("~/Views/Client/Catalog.cshtml", userGames.Adapt<List<GameDto>>());
        }
    }
}