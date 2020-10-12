using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Internals.Repository.Models;

namespace Warehouse.Domain.Internals.Repository.DataAccess
{
    internal class WarehouseDbContext : DbContext
    {
        public DbSet<Article> Articles { get; set; }
        public DbSet<Product> Products { get; set; }

        public WarehouseDbContext() { }

        public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options)
            : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=warehouse.sqlite");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>()
                .ToTable("articles")
                .HasKey(a => a.ArticleId);

            modelBuilder.Entity<Article>()
                .Property(a => a.Name)
                .HasColumnType("varchar(200)")
                .HasMaxLength(200);

            modelBuilder.Entity<Product>()
                .ToTable("products")
                .HasKey(p => p.Name);

            modelBuilder.Entity<Product>()
                .Property(p => p.Name)
                .HasColumnType("varchar(200)")
                .HasMaxLength(200);

            modelBuilder.Entity<Product>()
                .OwnsMany(p => p.ProductArticles, pa =>
                {
                    pa.ToTable("productarticles");
                    pa.WithOwner().HasForeignKey("Name");
                    pa.Property<int>("ProductArticleId");
                    pa.HasKey("ProductArticleId");
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}
