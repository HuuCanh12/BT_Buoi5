using Microsoft.EntityFrameworkCore;
using BT_Buoi5.Models;

namespace BT_Buoi5.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Grade> Grades { get; set; }

        // Add your DbSet properties here for your models
        // Example: public DbSet<YourModel> YourModels { get; set; }
    }
} 