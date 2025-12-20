namespace Gauniv.WebServer.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveGameFileAsync(IFormFile file, int gameId);
        Task<string> SaveCoverImageAsync(IFormFile file, int gameId);
        Task DeleteFileAsync(string filePath);
    }

    public class FileStorageService(IWebHostEnvironment environment) : IFileStorageService
    {
        private const string GameFilesFolder = "uploads/games/files";
        private const string CoverImagesFolder = "uploads/games/covers";

        public async Task<string> SaveGameFileAsync(IFormFile file, int gameId)
        {
            var folderPath = Path.Combine(environment.WebRootPath, GameFilesFolder);
            Directory.CreateDirectory(folderPath);

            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var fileName = $"{gameId}_{timestamp}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/{GameFilesFolder}/{fileName}".Replace("\\", "/");
        }

        public async Task<string> SaveCoverImageAsync(IFormFile file, int gameId)
        {
            var folderPath = Path.Combine(environment.WebRootPath, CoverImagesFolder);
            Directory.CreateDirectory(folderPath);

            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{gameId}_{timestamp}_cover{extension}";
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/{CoverImagesFolder}/{fileName}".Replace("\\", "/");
        }

        public Task DeleteFileAsync(string filePath)
        {
            var fullPath = Path.Combine(environment.WebRootPath, filePath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return Task.CompletedTask;
        }
    }
}