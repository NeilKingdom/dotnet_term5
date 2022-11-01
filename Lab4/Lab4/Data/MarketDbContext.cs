using Lab4.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab4.Data
{
    public class MarketDbContext : DbContext
    {
        public MarketDbContext(DbContextOptions<MarketDbContext> options) 
            : base(options)
        {}

        public DbSet<Client> Clients { get; set; }
        public DbSet<Brokerage> Brokerages { get; set; }
        public DbSet<Subscription> Subscriptions { get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>().ToTable("Client");
            modelBuilder.Entity<Brokerage>().ToTable("Broker");
            modelBuilder.Entity<Subscription>().ToTable("Subscription");

            // Add composite keys ClientID and BrokerageID to Subscriptions
            modelBuilder.Entity<Subscription>().HasKey(c => new { c.ClientID, c.BrokerageID });
        }
    }
}
