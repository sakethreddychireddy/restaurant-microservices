using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuService.Domain.Entities
{
    public class MenuItem
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public string Category { get; private set; } = string.Empty;
        public string Emoji { get; private set; } = "🍽️";
        public bool IsVegetarian { get; private set; }
        public string? Badge { get; private set; }
        public bool IsAvailable { get; private set; } = true;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
        private MenuItem() { }
        public static MenuItem Create(string name, string description, decimal price, string category, string emoji, 
            bool isVegetarian, string? badge = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
            if(price < 0)
                throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");
            return new MenuItem
            {
                Name = name,
                Description = description,
                Price = price,
                Category = category.ToLowerInvariant(),
                Emoji = emoji,
                IsVegetarian = isVegetarian,
                Badge = badge,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }
        public void Update(string? name, string? description, decimal? price,
         string? category, string? emoji, bool? isVegetarian, string? badge, bool? isAvailable)
        {
            if (name is not null) Name = name;
            if (description is not null) Description = description;
            if (price is not null) Price = price.Value;
            if (category is not null) Category = category.ToLowerInvariant();
            if (emoji is not null) Emoji = emoji;
            if (isVegetarian is not null) IsVegetarian = isVegetarian.Value;
            if (badge is not null) Badge = badge;
            if (isAvailable is not null) IsAvailable = isAvailable.Value;
            UpdatedAt = DateTime.UtcNow;
        }
        public void Disable() { IsAvailable = false; UpdatedAt = DateTime.UtcNow; }
    }
}
