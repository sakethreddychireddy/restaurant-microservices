using Microsoft.AspNetCore.Http;

namespace MenuService.Application.DTOs
{
    public class CreateMenuItemDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public IFormFile? ImageFile { get; set; }
        public bool IsVegetarian { get; set; }
        public string? Badge { get; set; }
    }
}
