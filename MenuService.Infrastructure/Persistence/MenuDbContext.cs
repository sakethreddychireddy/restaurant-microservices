using MenuService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MenuService.Infrastructure.Persistence
{
    public class MenuDbContext : DbContext
    {
        public MenuDbContext(DbContextOptions<MenuDbContext> options) : base(options)
        {
        }
        public DbSet<MenuItem> MenuItems => Set<MenuItem>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MenuDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
