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
        
        public async Task<IActionResult> Index()
        {
            var local_totalGames = await _applicationDbContext.Games.CountAsync();
            var local_totalCategories = await _applicationDbContext.Categories.CountAsync();
            var local_totalUsers = await _applicationDbContext.Users.CountAsync();
            
            var local_recentGames = await _applicationDbContext.Games
                .OrderByDescending(g => g.CreatedAt)
                .Take(5)
                .ToListAsync();

            var local_viewModel = new AdminDashboardViewModel
            {
                TotalGames = local_totalGames,
                TotalCategories = local_totalCategories,
                TotalUsers = local_totalUsers,
                RecentGames = local_recentGames.Adapt<List<GameDto>>()
            };

            return View(local_viewModel);
        }
        
        public async Task<IActionResult> Games(string searchTerm, List<int> categoryIds, int page = 1)
        {
            const int pageSize = 10;
            
            var categories = await _applicationDbContext.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();
            
            var local_query = _applicationDbContext.Games.AsQueryable();

            // Filter by search term
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                local_query = local_query.Where(g => g.Name.Contains(searchTerm));
            }

            // Filter by categories
            if (categoryIds != null && categoryIds.Any())
            {
                local_query = local_query.Where(g => g.GameCategories.Any(c => categoryIds.Contains(c.Id)));
            }

            var local_totalGames = await local_query.CountAsync();
            var local_totalPages = (int)Math.Ceiling(local_totalGames / (double)pageSize);
            
            var games = await local_query
                .OrderByDescending(g => g.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            GameViewModel local_gameViewModel = new GameViewModel()
            {
                GamesDtos = games.Adapt<List<GameDto>>(),
                CategoriesDtos = categories.Adapt<List<CategoryDto>>(),
                CurrentPage = page,
                TotalPages = local_totalPages,
                SearchTerm = searchTerm,
                SelectedCategoryIds = categoryIds ?? new List<int>()
            };
            
            return View("~/Views/Admin/Games/Index.cshtml",  local_gameViewModel);
        }
        
        public async Task<IActionResult> Categories(string searchTerm, int page = 1)
        {
            const int pageSize = 10;
            
            var local_query = _applicationDbContext.Categories.AsQueryable();

            // Filter by search term
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                local_query = local_query.Where(c => c.Name.Contains(searchTerm));
            }

            var local_totalCategories = await local_query.CountAsync();
            var local_totalPages = (int)Math.Ceiling(local_totalCategories / (double)pageSize);
            
            var categories = await local_query
                .OrderBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var local_viewModel = new CategoryViewModel
            {
                CategoriesDtos = categories.Adapt<List<CategoryDto>>(),
                CurrentPage = page,
                TotalPages = local_totalPages,
                SearchTerm = searchTerm ?? string.Empty
            };
            
            return View("~/Views/Admin/Categories/Index.cshtml", local_viewModel);
        }
        
    }
}