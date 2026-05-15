using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using RetailingApp.Entities;

public class AppDbContext: DbContext
{
    public AppDbContext() {}
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<Manufacturer> Manufacturers{get; set;}
    public DbSet<Category> Categories {get; set;}
    public DbSet<Location> Locations {get; set;} 
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)  => options.UseSqlite(@"Data Source=store.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
         
         modelBuilder.Entity<Product>().HasKey(p => p.Id);
         modelBuilder.Entity<Category>().HasKey(c => c.Id);
         modelBuilder.Entity<Manufacturer>().HasKey(m => m.Id);
         modelBuilder.Entity<Location>().HasKey(l => l.Id);
         modelBuilder.Entity<Category>().HasMany(c => c.Products).WithMany(p => p.Categories);
         modelBuilder.Entity<Manufacturer>().HasMany(m => m.Products).WithOne(p => p.Manufacturer);
         modelBuilder.Entity<Location>().HasMany(l => l.Products).WithOne(p => p.Location).HasForeignKey(p => p.LocationId);
    }

}

