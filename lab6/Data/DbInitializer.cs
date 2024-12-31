using Microsoft.EntityFrameworkCore;
using Lab6.Models;

namespace Lab6.Data
{
    public class DbInitializer
    {
        public static void Initialize(StudentDbContext context)
        {
            context.Database.Migrate();

            if (context.Students.Any())
            {
                return; // DB has been seeded
            }

            var students = new List<Student>
            {
                new Student { FirstName = "Carson",     LastName = "Alexander",     Program = "ICT" },
                new Student { FirstName = "Merideth",   LastName = "Alonso",        Program = "ICT" },
                new Student { FirstName = "Arturo",     LastName = "Anand",         Program = "ICT" },
                new Student { FirstName = "Gytis",      LastName = "Barzdukas",     Program = "ICT" }
            };

            students.ForEach(student => context.Students.Add(student));
            context.SaveChanges();
        }
    }
}
