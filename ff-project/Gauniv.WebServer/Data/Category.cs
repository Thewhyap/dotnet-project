using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Gauniv.WebServer.Data
{
    public class Category {
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public IEnumerable<Game> CategoryGames { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime LastUpdatedAt { get; set; }
        
    }
}