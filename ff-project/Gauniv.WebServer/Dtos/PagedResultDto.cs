namespace Gauniv.WebServer.Dtos
{
    public class PagedResultDto<T>
    {
        
        public List<T> Items { get; set; }
        public int Total { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; }
        
    }
}