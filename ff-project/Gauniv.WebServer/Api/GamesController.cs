#region Licence
// Cyril Tisserand
// Projet Gauniv - WebServer
// Gauniv 2025
// 
// Licence MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software
// and associated documentation files (the “Software”), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
// Any new method must be in a different namespace than the previous ones
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions: 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software. 
// The Software is provided “as is”, without warranty of any kind, express or implied,
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
using Gauniv.WebServer.Data;
using Gauniv.WebServer.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Text;
using CommunityToolkit.HighPerformance.Memory;
using CommunityToolkit.HighPerformance;
using Gauniv.WebServer.Dtos.Category;
using Gauniv.WebServer.Dtos.Game;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using MapsterMapper;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Gauniv.WebServer.Api
{
    [Route("api/1.0.0/[controller]/[action]")]
    [ApiController]
    public class GamesController(
        ApplicationDbContext appDbContext, 
        IMapper mapper, 
        UserManager<User> userManager, 
        MappingProfile mp,
        IWebHostEnvironment environment) : ControllerBase
    {
        
        [AllowAnonymous]
        public async Task<ActionResult<List<CategoryDto>>> Categories()
        {
            
            var local_categories = await appDbContext.Categories.ToListAsync();
            
            return Ok(mapper.Map<List<CategoryDto>>(local_categories));
            
        }
        
        
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResultDto<GameDto>>> Game(
            [FromQuery] int offset = 0,
            [FromQuery] int limit = 10,
            [FromQuery] int[]? category = null)
        {
            IQueryable<Game> local_query;

            // Si l'utilisateur est connecté, retourner uniquement ses jeux (comme CatalogController)
            if (User.Identity?.IsAuthenticated == true)
            {
                var local_user = await userManager.GetUserAsync(User);
                if (local_user != null)
                {
                    // Exactement la même logique que CatalogController
                    var local_userGames = await appDbContext.Users
                        .Where(u => u.Id == local_user.Id)
                        .SelectMany(u => u.UserGames)
                        .Include(g => g.GameCategories)
                        .ToListAsync();
                    
                    local_query = local_userGames.AsQueryable();
                }
                else
                {
                    local_query = appDbContext.Games
                        .Include(g => g.GameCategories)
                        .AsQueryable();
                }
            }
            else
            {
                // Utilisateur non connecté : tous les jeux
                local_query = appDbContext.Games
                    .Include(g => g.GameCategories)
                    .AsQueryable();
            }

            // Filtre par catégories
            if (category != null && category.Length > 0)
            {
                local_query = local_query.Where(g => g.GameCategories.Any(c => category.Contains(c.Id)));
            }

            var local_total = local_query.Count();

            var local_games = local_query
                .Skip(offset)
                .Take(limit)
                .Select(g => new GameDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    Price = g.Price,
                    CoverImage = g.CoverImage,
                    CategoryIds = g.GameCategories.Select(c => c.Id).ToList()
                })
                .ToList();

            return Ok(new PagedResultDto<GameDto>
            {
                Items = local_games,
                Total = local_total,
                Offset = offset,
                Limit = limit
            });
        }
        
    
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Download([FromQuery] int id)
        {
            var local_user = await userManager.GetUserAsync(User);
            if (local_user == null)
                return Unauthorized();

            // Vérifier que l'utilisateur possède le jeu
            var local_game = await appDbContext.Games
                .Include(g => g.GameUsers)
                .FirstOrDefaultAsync(g => g.Id == id && g.GameUsers.Any(u => u.Id == local_user.Id));

            if (local_game == null)
                return Forbid("You don't own this game");

            // Construire le chemin absolu du fichier
            var local_relativePath = local_game.Payload?.TrimStart('/') ?? "";
            var local_absolutePath = Path.Combine(environment.WebRootPath, local_relativePath);

            // Vérifier que le fichier existe
            if (string.IsNullOrEmpty(local_game.Payload) || !System.IO.File.Exists(local_absolutePath))
                return NotFound("Game binary not found");

            // Streamer le fichier directement depuis le disque
            // STREAMING EFFICACE : Seulement 80 KB en mémoire à la fois
            var local_stream = new FileStream(
                local_absolutePath, 
                FileMode.Open, 
                FileAccess.Read, 
                FileShare.Read,
                bufferSize: 81920, // 80 KB buffer - optimisé pour les gros fichiers
                useAsync: true
            );
            
            var local_fileExtension = Path.GetExtension(local_game.Payload);
            var local_fileName = $"{local_game.Name}{local_fileExtension}";

            // enableRangeProcessing: permet les téléchargements partiels/reprise
            return File(
                local_stream, 
                "application/octet-stream", 
                local_fileName,
                enableRangeProcessing: true
            );
        }
        
    }
    
    
}
