using Gauniv.WebServer.Data;
using Gauniv.WebServer.Dtos.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gauniv.WebServer.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class AdminController(ApplicationDbContext applicationDbContext) :  Controller
    {
        
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;
        
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Games()
        {
            return View("~/Views/Admin/Games/Index.cshtml");
        }
        
        public async Task<IActionResult> Categories()
        {
            var categories = await _applicationDbContext.Categories.ToListAsync();
            return View("~/Views/Admin/Categories/Index.cshtml", categories);
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
        public async Task<IActionResult> CreateCategory(CreateCategoryDto createCategoryDto)
        {
            if (!ModelState.IsValid)
            {
                // TODO : redirect
                return View("~/Views/Admin/Categories/Create.cshtml", createCategoryDto);
            }
            
            var local_categoryExists = await _applicationDbContext.Categories
                .AnyAsync(c => c.Name.ToLower() == createCategoryDto.Name.ToLower());

            if (local_categoryExists)
            {
                ModelState.AddModelError("Name", "A category with this name already exists.");
                return View("~/Views/Admin/Categories/Create.cshtml", createCategoryDto);
            }

            try
            {
                var local_category = new Category
                {
                    Name = createCategoryDto.Name,
                    Description = createCategoryDto.Description,
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
                return View("~/Views/Admin/Categories/Create.cshtml", createCategoryDto);
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
        
    }
    
}