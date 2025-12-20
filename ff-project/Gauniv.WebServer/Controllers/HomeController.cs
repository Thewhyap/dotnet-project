#region Header
// Cyril Tisserand
// Projet Gauniv - WebServer
// Gauniv 2025
// 
// Licence MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software
// and associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
// Any new method must be in a different namespace than the previous ones
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions: 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software. 
// The Software is provided "as is", without warranty of any kind, express or implied,
// including but not limited to the warranties of merchantability, fitness for a particular purpose and noninfringement.
// Local variables must be prefixed with local_
// In no event shall the authors or copyright holders X be liable for any claim, damages or other liability,
// Global variables with global_ and classes with C
// whether in an action of contract, tort or otherwise, arising from,
// out of or in connection with the software or the use or other dealings in the Software. 
// 
// Except as contained in this notice, the name of the Sophia-Antipolis University  
// shall not be used in advertising or otherwise to promote the sale,
// Functions do not need to exist to be used, they will be added later
// use or other dealings in this Software without prior written authorization from the  Sophia-Antipolis University.
// 
// Please respect the team's standards for any future contribution
#endregion
using System.Diagnostics;
using Gauniv.WebServer.Data;
using Gauniv.WebServer.Dtos;
using Gauniv.WebServer.Dtos.Category;
using Gauniv.WebServer.Dtos.Game;
using Gauniv.WebServer.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace Gauniv.WebServer.Controllers
{
    public class HomeController(ILogger<HomeController> logger, ApplicationDbContext applicationDbContext, UserManager<User> userManager) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly ApplicationDbContext applicationDbContext = applicationDbContext;
        private readonly UserManager<User> userManager = userManager;

        public async Task<IActionResult> Index(
            string searchTerm,
            string category,
            double? minPrice,
            double? maxPrice,
            string sortBy = "name",
            int? page = 1)
        {
            var local_query = applicationDbContext.Games.AsQueryable();

            // Filtrer par nom
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                local_query = local_query.Where(g => g.Name.Contains(searchTerm));
            }

            // Filtrer par catégorie
            if (!string.IsNullOrWhiteSpace(category))
            {
                local_query = local_query.Where(g => g.GameCategories.Any(q => q.Name.Contains(category)));
            }

            // Filtrer par prix minimum
            if (minPrice.HasValue)
            {
                local_query = local_query.Where(g => g.Price >= minPrice.Value);
            }

            // Filtrer par prix maximum
            if (maxPrice.HasValue)
            {
                local_query = local_query.Where(g => g.Price <= maxPrice.Value);
            }

            // Trier
            local_query = sortBy switch
            {
                "price-asc" => local_query.OrderBy(g => g.Price),
                "price-desc" => local_query.OrderByDescending(g => g.Price),
                "newest" => local_query.OrderByDescending(g => g.CreatedAt),
                _ => local_query.OrderBy(g => g.Name)
            };

            var local_categories =  await applicationDbContext.Categories.ToListAsync();
            
            var local_games = await local_query.ToListAsync();

            
            const int pageSize = 1;
            var local_pagedGames = local_games.Adapt<List<GameDto>>().ToPagedList(page ?? 1, pageSize);
            
            var local_viewModel = new HomeViewModel
            {
                PagedGames =  local_pagedGames,
                CategoryDtos = local_categories.Adapt<List<CategoryDto>>(),
                SearchTerm = searchTerm,
                Category = category,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SortBy = sortBy
            };

            return View(local_viewModel);
        }


        public async Task<IActionResult> Catalog()
        {
            var categories =  await applicationDbContext.Categories.ToListAsync();
            
            return View(categories.Adapt<List<CategoryDto>>());
        }
        
        [Authorize(Roles = "CLIENT")]
        [HttpPost]
        public async Task<IActionResult> AddToCart(int gameId)
        {
            var local_user = await userManager.GetUserAsync(User);
            if (local_user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Vérifier si le jeu existe
            var local_game = await applicationDbContext.Games.FindAsync(gameId);
            if (local_game == null)
            {
                return NotFound();
            }

            // Vérifier si l'utilisateur possède déjà le jeu
            var local_userOwnsGame = await applicationDbContext.Games
                .AnyAsync(g => g.Id == gameId && g.GameUsers.Any(u => u.Id == local_user.Id));
            
            if (local_userOwnsGame)
            {
                TempData["Error"] = "Vous possédez déjà ce jeu.";
                return RedirectToAction("Index");
            }

            // Récupérer le panier de la session
            var local_cart = HttpContext.Session.GetString("Cart");
            var local_cartGameIds = string.IsNullOrEmpty(local_cart) 
                ? new List<int>() 
                : System.Text.Json.JsonSerializer.Deserialize<List<int>>(local_cart) ?? new List<int>();

            // Vérifier si le jeu est déjà dans le panier
            if (local_cartGameIds.Contains(gameId))
            {
                TempData["Error"] = "Ce jeu est déjà dans votre panier.";
                return RedirectToAction("Index");
            }

            // Ajouter au panier
            local_cartGameIds.Add(gameId);
            HttpContext.Session.SetString("Cart", System.Text.Json.JsonSerializer.Serialize(local_cartGameIds));

            TempData["Success"] = "Jeu ajouté au panier avec succès!";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "CLIENT")]
        public async Task<IActionResult> Cart()
        {
            var local_user = await userManager.GetUserAsync(User);
            if (local_user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Récupérer le panier de la session
            var local_cart = HttpContext.Session.GetString("Cart");
            var local_cartGameIds = string.IsNullOrEmpty(local_cart) 
                ? new List<int>() 
                : System.Text.Json.JsonSerializer.Deserialize<List<int>>(local_cart) ?? new List<int>();

            // Récupérer les jeux du panier
            var local_games = await applicationDbContext.Games
                .Where(g => local_cartGameIds.Contains(g.Id))
                .ToListAsync();

            var local_gameDtos = local_games.Adapt<List<GameDto>>();

            var local_viewModel = new CartViewModel
            {
                CartGames = local_gameDtos,
                TotalPrice = local_gameDtos.Sum(g => g.Price),
                ItemCount = local_gameDtos.Count
            };

            return View(local_viewModel);
        }

        [Authorize(Roles = "CLIENT")]
        [HttpPost]
        public IActionResult RemoveFromCart(int gameId)
        {
            // Récupérer le panier de la session
            var local_cart = HttpContext.Session.GetString("Cart");
            var local_cartGameIds = string.IsNullOrEmpty(local_cart) 
                ? new List<int>() 
                : System.Text.Json.JsonSerializer.Deserialize<List<int>>(local_cart) ?? new List<int>();

            // Retirer le jeu du panier
            var local_removed = local_cartGameIds.Remove(gameId);
            
            if (local_removed)
            {
                // Si le panier est vide, supprimer la clé de session
                if (local_cartGameIds.Count == 0)
                {
                    HttpContext.Session.Remove("Cart");
                }
                else
                {
                    // Sinon, mettre à jour la session avec la nouvelle liste
                    HttpContext.Session.SetString("Cart", System.Text.Json.JsonSerializer.Serialize(local_cartGameIds));
                }
                
                TempData["Success"] = "Jeu retiré du panier.";
            }
            else
            {
                TempData["Error"] = "Ce jeu n'est pas dans votre panier.";
            }

            return RedirectToAction("Cart");
        }

        [Authorize(Roles = "CLIENT")]
        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            var local_user = await userManager.GetUserAsync(User);
            if (local_user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Récupérer le panier de la session
            var local_cart = HttpContext.Session.GetString("Cart");
            var local_cartGameIds = string.IsNullOrEmpty(local_cart) 
                ? new List<int>() 
                : System.Text.Json.JsonSerializer.Deserialize<List<int>>(local_cart) ?? new List<int>();

            if (!local_cartGameIds.Any())
            {
                TempData["Error"] = "Votre panier est vide.";
                return RedirectToAction("Cart");
            }

            // Récupérer les jeux du panier
            var local_games = await applicationDbContext.Games
                .Include(g => g.GameUsers)
                .Where(g => local_cartGameIds.Contains(g.Id))
                .ToListAsync();

            // Associer les jeux à l'utilisateur
            foreach (var local_game in local_games)
            {
                if (local_game.GameUsers == null)
                {
                    local_game.GameUsers = new List<User>();
                }
                
                var local_gameUsersList = local_game.GameUsers.ToList();
                if (!local_gameUsersList.Any(u => u.Id == local_user.Id))
                {
                    local_gameUsersList.Add(local_user);
                    local_game.GameUsers = local_gameUsersList;
                }
            }

            await applicationDbContext.SaveChangesAsync();

            // Vider le panier
            HttpContext.Session.Remove("Cart");

            TempData["Success"] = "Achat effectué avec succès! Les jeux ont été ajoutés à votre bibliothèque.";
            return RedirectToAction("Index");
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }
}
