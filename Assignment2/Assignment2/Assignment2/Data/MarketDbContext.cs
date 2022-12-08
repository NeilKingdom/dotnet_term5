using Assignment2.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment2.Data
{
    public class MarketDbContext : DbContext
    {
        public MarketDbContext(DbContextOptions<MarketDbContext> options) : base(options)
        {}

        public DbSet<Client> Clients { get; set; }
        public DbSet<Brokerage> Brokerages { get; set; }
        public DbSet<Subscription> Subscriptions { get; set;}
        public DbSet<Advertisement> Advertisements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>().ToTable("Client");
            modelBuilder.Entity<Brokerage>().ToTable("Broker");
            modelBuilder.Entity<Subscription>().ToTable("Subscription");
            modelBuilder.Entity<Advertisement>().ToTable("Advertisement");

            // Add composite keys ClientID and BrokerageID to Subscriptions
            modelBuilder.Entity<Subscription>().HasKey(c => new { c.ClientId, c.BrokerageId });
        }
    }
}
