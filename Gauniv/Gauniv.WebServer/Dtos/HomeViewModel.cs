using Gauniv.WebServer.Dtos.Category;
using Gauniv.WebServer.Dtos.Game;
using X.PagedList;

namespace Gauniv.WebServer.Dtos
{
    public class HomeViewModel
    {
        public List<GameDto> GamesDtos { get; set; } = new();
        
        public List<CategoryDto> CategoryDtos { get; set; } = new();
        public IPagedList<GameDto> PagedGames { get; set; }
        public string SearchTerm { get; set; }
        public string Category { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
        public string SortBy { get; set; } = "name";
        public bool? Owned { get; set; }
        public long? MinSize { get; set; }
        public long? MaxSize { get; set; }
    }
}