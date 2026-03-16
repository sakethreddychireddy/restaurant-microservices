using MenuService.Application.Interfaces;
using Microsoft.Extensions.Hosting;

namespace MenuService.Infrastructure.Storage
{
    public class LocalImageStorageService : IImageStorageService
    {
        private readonly IHostEnvironment _env;

        public LocalImageStorageService(IHostEnvironment env)
        {
            _env = env;
        }

        private string WebRootPath =>
            Path.Combine(_env.ContentRootPath, "wwwroot");

        public async Task<string> SaveImageAsync(
            Guid itemId,
            Stream imageStream,
            string contentType,
            CancellationToken ct = default)
        {
            var ext = contentType.ToLower() switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/webp" => ".webp",
                _ => throw new InvalidOperationException(
                    "Only JPEG, PNG and WebP images are allowed.")
            };

            var uploadsFolder = Path.Combine(WebRootPath, "images", "menu");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{itemId}{ext}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await imageStream.CopyToAsync(stream, ct);

            return $"/images/menu/{fileName}";
        }

        public Task DeleteImageAsync(
            string imageUrl,
            CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return Task.CompletedTask;

            var filePath = Path.Combine(
                WebRootPath, imageUrl.TrimStart('/'));

            if (File.Exists(filePath))
                File.Delete(filePath);

            return Task.CompletedTask;
        }
    }
}