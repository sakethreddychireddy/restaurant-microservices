using Microsoft.Extensions.Logging;
using OrderService.Application.Interfaces;
using System.Net.Http.Json;

namespace OrderService.Infrastructure.Http
{
    public class MenuServiceHttpClient : IMenuServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MenuServiceHttpClient> _logger;

        public MenuServiceHttpClient(HttpClient httpClient, ILogger<MenuServiceHttpClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<MenuItemInfo>> GetMenuItemsAsync(
            IEnumerable<Guid> ids, CancellationToken ct = default)
        {
            var result = new List<MenuItemInfo>();

            foreach (var id in ids)
            {
                _logger.LogInformation("Requesting menu item with ID '{Id}' from Menu Service.", id);

                var response = await _httpClient.GetAsync($"api/menuitems/{id}", ct);

                _logger.LogInformation("Status Code: {StatusCode} for item ID '{Id}'.",
                    response.StatusCode, id);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to fetch menu item with ID '{Id}'. Status: {StatusCode}",
                        id, response.StatusCode);
                    throw new InvalidOperationException($"Menu item '{id}' not found.");
                }

                var item = await response.Content.ReadFromJsonAsync<MenuItemResponse>(
                    cancellationToken: ct);

                if (item is not null)
                {
                    _logger.LogInformation("Fetched menu item '{Name}' with ID '{Id}'.",
                        item.Name, item.Id);
                    result.Add(new MenuItemInfo(item.Id, item.Name, item.Price, item.IsAvailable));
                }
            }

            return result;
        }

        private record MenuItemResponse(
            Guid Id,
            string Name,
            string Description,
            decimal Price,
            string Category,
            string Emoji,
            bool IsVegetarian,
            string? Badge,
            bool IsAvailable,
            DateTime CreatedAt,
            DateTime UpdatedAt
        );
    }
}