namespace MenuService.Application.Interfaces
{
    public interface IImageStorageService
    {
        Task<string> SaveImageAsync(Guid itemId, Stream imageStream,
            string contentType, CancellationToken ct = default);
        Task DeleteImageAsync(string imageUrl,
            CancellationToken ct = default);
    }
}
