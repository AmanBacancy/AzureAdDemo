using Microsoft.EntityFrameworkCore;

namespace AzureAdDemo.Data
{
    public class AzureAdDemoDbContext : DbContext
    {
        public AzureAdDemoDbContext(DbContextOptions<AzureAdDemoDbContext> options) : base(options)
        { }
        public DbSet<Role> Role { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserRoleMapping> UserRoleMapping { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
        }
    }
}
