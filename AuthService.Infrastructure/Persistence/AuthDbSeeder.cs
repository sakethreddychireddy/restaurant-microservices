using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Infrastructure.Persistence
{
    public static class AuthDbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
            var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            if (!db.Users.Any())
            {
                var admin = User.Create("Admin", "mithila.restaurant14@gmail.com", hasher.Hash("MithilaRestaurant@14"), "Admin");
                db.Users.Add(admin);
                await db.SaveChangesAsync();
            }
        }
    }
}
