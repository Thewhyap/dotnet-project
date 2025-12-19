using Gauniv.WebServer.Data;
using Gauniv.WebServer.Dtos.Category;
using Gauniv.WebServer.Dtos.Game;
using Gauniv.WebServer.Models;
using Gauniv.WebServer.Services;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gauniv.WebServer.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class AdminController(ApplicationDbContext applicationDbContext, IFileStorageService fileStorageService) :  Controller
    {
        
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;
    
        private readonly IFileStorageService _fileStorageService = fileStorageService;
        
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
        
        
        // Categories
        
        [Route("Admin/Categories/Create")]
        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View("~/Views/Admin/Categories/Create.cshtml");
        }

        [Route("Admin/Categories/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(CategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                // TODO : redirect
                return View("~/Views/Admin/Categories/Create.cshtml", categoryDto);
            }
            
            var local_categoryExists = await _applicationDbContext.Categories
                .AnyAsync(c => c.Name.ToLower() == categoryDto.Name.ToLower());

            if (local_categoryExists)
            {
                ModelState.AddModelError("Name", "A category with this name already exists.");
                return View("~/Views/Admin/Categories/Create.cshtml", categoryDto);
            }

            try
            {
                var local_category = new Category
                {
                    Name = categoryDto.Name,
                    Description = categoryDto.Description,
                    CreatedAt = DateTime.UtcNow,
                    LastUpdatedAt = DateTime.UtcNow
                };

                _applicationDbContext.Categories.Add(local_category);
                await _applicationDbContext.SaveChangesAsync();
                
                TempData["Success"] = $"Category '{local_category.Name}' created successfully!";

                return RedirectToAction("Categories", "Admin");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the category. Please try again.");
                return View("~/Views/Admin/Categories/Create.cshtml", categoryDto);
            }
            
        }
        
        [Route("Admin/Categories/Update/{id:int}")]
        [HttpGet]
        public async Task<IActionResult> UpdateCategory(int id)
        {
            var category = await _applicationDbContext.Categories.FindAsync(id);
    
            if (category == null)
            {
                TempData["Error"] = "Category not found.";
                return RedirectToAction("Categories");
            }

            var updateCategoryDto = new UpdateCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };

            return View("~/Views/Admin/Categories/Update.cshtml", updateCategoryDto);
        }
        
        [Route("Admin/Categories/Update/{id:int}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto updateCategoryDto)
        {
            if (id != updateCategoryDto.Id)
            {
                TempData["Error"] = "Invalid category ID.";
                return RedirectToAction("Categories");
            }

            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/Categories/Update.cshtml", updateCategoryDto);
            }

            var category = await _applicationDbContext.Categories.FindAsync(id);

            if (category == null)
            {
                TempData["Error"] = "Category not found.";
                return RedirectToAction("Categories");
            }

            var local_categoryExists = await _applicationDbContext.Categories
                .AnyAsync(c => c.Name.ToLower() == updateCategoryDto.Name.ToLower() && c.Id != id);

            if (local_categoryExists)
            {
                ModelState.AddModelError("Name", "A category with this name already exists.");
                return View("~/Views/Admin/Categories/Update.cshtml", updateCategoryDto);
            }

            try
            {
                category.Name = updateCategoryDto.Name;
                category.Description = updateCategoryDto.Description;
                category.LastUpdatedAt = DateTime.UtcNow;

                _applicationDbContext.Categories.Update(category);
                await _applicationDbContext.SaveChangesAsync();

                TempData["Success"] = $"Category '{category.Name}' updated successfully!";

                return RedirectToAction("Categories");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the category. Please try again.");
                return View("~/Views/Admin/Categories/Update.cshtml", updateCategoryDto);
            }
        }
        
        [Route("Admin/Categories/Delete/{id:int}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _applicationDbContext.Categories.FindAsync(id);

            if (category == null)
            {
                TempData["Error"] = "Category not found.";
                return RedirectToAction("Categories");
            }

            try
            {
                _applicationDbContext.Categories.Remove(category);
                await _applicationDbContext.SaveChangesAsync();

                TempData["Success"] = $"Category '{category.Name}' deleted successfully!";

                return RedirectToAction("Categories");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while deleting the category. Please try again.";
                return RedirectToAction("Categories");
            }
        }
        
        
        // Games
        
        [Route("Admin/Games/Create")]
        [HttpGet]
        public async Task<IActionResult> CreateGame()
        {
            var categories = await _applicationDbContext.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

            var CreateGameViewModel = new GameCreateViewModel()
            {
                CategoriesDtos = categories.Adapt<List<CategoryDto>>()
            };

            return View("~/Views/Admin/Games/Create.cshtml",  CreateGameViewModel);
        }
        
        
        [Route("Admin/Games/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGame(GameCreateViewModel gameViewModel)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _applicationDbContext.Categories
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                var viewModel = new GameCreateViewModel();
                viewModel.CategoriesDtos = categories.Adapt<List<CategoryDto>>();
                
                return View("~/Views/Admin/Games/Create.cshtml", viewModel);
            }

            try
            {
                // Mapping of DTO to entity
                var game = gameViewModel.GameCreateDto.Adapt<Game>();

                // temp ID saving
                _applicationDbContext.Games.Add(game);
                await _applicationDbContext.SaveChangesAsync();
                
                game.Payload = await _fileStorageService.SaveGameFileAsync(gameViewModel.GameCreateDto.Payload, game.Id);
                
                game.CoverImage = await _fileStorageService.SaveCoverImageAsync(gameViewModel.GameCreateDto.CoverImage, game.Id);
                
                game.GameCategories = await _applicationDbContext.Categories
                    .Where(c => gameViewModel.GameCreateDto.CategoryIds.Contains(c.Id))
                    .ToListAsync();
                
                _applicationDbContext.Games.Update(game);
                await _applicationDbContext.SaveChangesAsync();

                TempData["Success"] = $"Game '{game.Name}' created successfully!";
                return RedirectToAction("Games");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while creating the game. Please try again.";
                
                var categories = await _applicationDbContext.Categories
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                var viewModel = new GameViewModel();
                viewModel.CategoriesDtos = categories.Adapt<List<CategoryDto>>();
                
                return View("~/Views/Admin/Games/Create.cshtml", viewModel);
            }
            
        }
        
    }
    
}