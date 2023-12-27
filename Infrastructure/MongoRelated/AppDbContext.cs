using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;

namespace mtfiddle;

public class AppDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Receive> Receives { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public static AppDbContext Create(IMongoDatabase database, MongoDbProvider initializer)
    {
        initializer.Initialize();
        return new(new DbContextOptionsBuilder<AppDbContext>().UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName).Options);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Receive>().ToCollection("Receives");
        modelBuilder.Entity<Order>().ToCollection("Orders");
        modelBuilder.Entity<Transaction>().ToCollection("Transactions");
        modelBuilder.Entity<Event>().ToCollection("Events");
    }
}