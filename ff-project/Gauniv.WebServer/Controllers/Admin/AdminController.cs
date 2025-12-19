using Gauniv.WebServer.Data;
using Gauniv.WebServer.Dtos.Category;
using Gauniv.WebServer.Dtos.Game;
using Gauniv.WebServer.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gauniv.WebServer.Controllers.Admin
{
    [Authorize(Roles = "ADMIN")]
    public class AdminController(ApplicationDbContext _applicationDbContext): Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }
        
        public async Task<IActionResult> Games()
        {
            var categories = await _applicationDbContext.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();
            
            var games = await _applicationDbContext.Games.ToListAsync();

            GameViewModel local_gameViewModel = new GameViewModel()
            {
                GamesDtos = games.Adapt<List<GameDto>>(),
                CategoriesDtos = categories.Adapt<List<CategoryDto>>(),
            };
            
            return View("~/Views/Admin/Games/Index.cshtml",  local_gameViewModel);
        }
        
        public async Task<IActionResult> Categories()
        {
            var categories = await _applicationDbContext.Categories.ToListAsync();
            var categoriesDtos = categories.Adapt<List<CategoryDto>>();
            return View("~/Views/Admin/Categories/Index.cshtml", categoriesDtos);
        }
        
    }
}