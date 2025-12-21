using System.ComponentModel.DataAnnotations;

namespace Gauniv.WebServer.Dtos.Game
{
    public class UpdateGameDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Game name is required")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        [Display(Name = "Game Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Update Game Image (Optional)")]
        public IFormFile? Image { get; set; }

        [Display(Name = "Update Game Files (Optional)")]
        public IFormFile? Payload { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value")]
        [Display(Name = "Price")]
        public double Price { get; set; }

        [Required(ErrorMessage = "At least one category is required")]
        [Display(Name = "Categories")]
        public List<int> CategoryIds { get; set; }
    }
}