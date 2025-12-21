using Gauniv.WebServer.Data;
using Gauniv.WebServer.Dtos.Category;
using Gauniv.WebServer.Dtos.Game;
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
        [DisableRequestSizeLimit] // Désactive toute limite de taille
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
                
                var (payloadPath, payloadSize) = await _fileStorageService.SaveGameFileAsync(gameViewModel.GameCreateDto.Payload, game.Id);
                game.Payload = payloadPath;
                game.Size = payloadSize;
                
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
        
        [Route("Admin/Games/Update/{id}")]
        [HttpGet]
        public async Task<IActionResult> UpdateGame(int id)
        {
            var local_game = await _applicationDbContext.Games
                .Include(g => g.GameCategories)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (local_game == null)
            {
                TempData["Error"] = "Game not found.";
                return RedirectToAction("Games", "Admin");
            }

            var local_categories = await _applicationDbContext.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

            var local_updateGameDto = new UpdateGameDto
            {
                Id = local_game.Id,
                Name = local_game.Name,
                Description = local_game.Description,
                Price = local_game.Price,
                CategoryIds = local_game.GameCategories?.Select(c => c.Id).ToList() ?? new List<int>()
            };

            var local_viewModel = new GameUpdateViewModel
            {
                UpdateGameDto = local_updateGameDto,
                CategoriesDtos = local_categories.Adapt<List<CategoryDto>>(),
                CurrentCoverImage = local_game.CoverImage
            };

            return View("~/Views/Admin/Games/Update.cshtml", local_viewModel);
        }

        [Route("Admin/Games/Update/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DisableRequestSizeLimit] // Désactive toute limite de taille
        public async Task<IActionResult> UpdateGame(int id, GameUpdateViewModel gameUpdateViewModel)
        {
            // Ensure the ID from route matches the DTO
            gameUpdateViewModel.UpdateGameDto.Id = id;

            if (!ModelState.IsValid)
            {
                var local_categories = await _applicationDbContext.Categories
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                gameUpdateViewModel.CategoriesDtos = local_categories.Adapt<List<CategoryDto>>();
                
                var local_game = await _applicationDbContext.Games.FindAsync(gameUpdateViewModel.UpdateGameDto.Id);
                if (local_game != null)
                {
                    gameUpdateViewModel.CurrentCoverImage = local_game.CoverImage;
                }
                
                return View("~/Views/Admin/Games/Update.cshtml", gameUpdateViewModel);
            }

            try
            {
                var local_game = await _applicationDbContext.Games
                    .Include(g => g.GameCategories)
                    .FirstOrDefaultAsync(g => g.Id == gameUpdateViewModel.UpdateGameDto.Id);

                if (local_game == null)
                {
                    TempData["Error"] = "Game not found.";
                    return RedirectToAction("Games", "Admin");
                }

                // Update basic properties
                local_game.Name = gameUpdateViewModel.UpdateGameDto.Name;
                local_game.Description = gameUpdateViewModel.UpdateGameDto.Description;
                local_game.Price = gameUpdateViewModel.UpdateGameDto.Price;

                // Update cover image if provided
                if (gameUpdateViewModel.UpdateGameDto.Image != null)
                {
                    // Delete old cover image
                    if (!string.IsNullOrEmpty(local_game.CoverImage))
                    {
                        await _fileStorageService.DeleteFileAsync(local_game.CoverImage);
                    }
                    
                    // Save new cover image
                    local_game.CoverImage = await _fileStorageService.SaveCoverImageAsync(gameUpdateViewModel.UpdateGameDto.Image, local_game.Id);
                }

                // Update payload if provided
                if (gameUpdateViewModel.UpdateGameDto.Payload != null)
                {
                    // Delete old game file
                    if (!string.IsNullOrEmpty(local_game.Payload))
                    {
                        await _fileStorageService.DeleteFileAsync(local_game.Payload);
                    }
                    
                    // Save new game file and update size
                    var (payloadPath, payloadSize) = await _fileStorageService.SaveGameFileAsync(gameUpdateViewModel.UpdateGameDto.Payload, local_game.Id);
                    local_game.Payload = payloadPath;
                    local_game.Size = payloadSize;
                }

                // Update categories
                local_game.GameCategories = await _applicationDbContext.Categories
                    .Where(c => gameUpdateViewModel.UpdateGameDto.CategoryIds.Contains(c.Id))
                    .ToListAsync();

                _applicationDbContext.Games.Update(local_game);
                await _applicationDbContext.SaveChangesAsync();

                TempData["Success"] = $"Game '{local_game.Name}' updated successfully!";
                return RedirectToAction("Games", "Admin");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while updating the game. Please try again.";
                
                var local_categories = await _applicationDbContext.Categories
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                gameUpdateViewModel.CategoriesDtos = local_categories.Adapt<List<CategoryDto>>();
                
                return View("~/Views/Admin/Games/Update.cshtml", gameUpdateViewModel);
            }
        }

        [Route("Admin/Games/Delete/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGame(int id)
        {
            try
            {
                var local_game = await _applicationDbContext.Games.FindAsync(id);

                if (local_game == null)
                {
                    TempData["Error"] = "Game not found.";
                    return RedirectToAction("Games", "Admin");
                }

                // Delete associated files before deleting the game
                if (!string.IsNullOrEmpty(local_game.CoverImage))
                {
                    await _fileStorageService.DeleteFileAsync(local_game.CoverImage);
                }

                if (!string.IsNullOrEmpty(local_game.Payload))
                {
                    await _fileStorageService.DeleteFileAsync(local_game.Payload);
                }

                _applicationDbContext.Games.Remove(local_game);
                await _applicationDbContext.SaveChangesAsync();

                TempData["Success"] = $"Game '{local_game.Name}' deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while deleting the game. Please try again.";
            }

            return RedirectToAction("Games", "Admin");
        }
    }
}