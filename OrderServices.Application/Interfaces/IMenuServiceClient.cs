namespace OrderService.Application.Interfaces
{
    public interface IMenuServiceClient
    {
        Task<IEnumerable<MenuItemInfo>> GetMenuItemsAsync(
            IEnumerable<Guid> ids, CancellationToken ct = default);
    }
    public record MenuItemInfo(Guid Id, string Name, decimal Price, bool IsAvailable);
}

