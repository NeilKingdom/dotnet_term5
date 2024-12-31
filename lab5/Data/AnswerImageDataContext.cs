using Lab5.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab5.Data
{
    public class AnswerImageDataContext : DbContext
    {
        public AnswerImageDataContext(DbContextOptions<AnswerImageDataContext> context) : base(context)
        {}

        public DbSet<AnswerImage> AnswerImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnswerImage>().ToTable("AnswerImage");
        }
    }
}
