using Gauniv.WebServer.Data;
using Gauniv.WebServer.Dtos.Category;
using Gauniv.WebServer.Models;
using Gauniv.WebServer.Services;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gauniv.WebServer.Controllers.Admin
{
    [Authorize(Roles = "ADMIN")]
    public class GameController(ApplicationDbContext _applicationDbContext, IFileStorageService _fileStorageService): Controller
    {
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
                return RedirectToAction("Games", "Admin");
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