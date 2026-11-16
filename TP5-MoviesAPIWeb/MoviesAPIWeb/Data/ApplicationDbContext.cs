
using Microsoft.EntityFrameworkCore;
using MoviesAPIWeb.Models;

namespace MoviesAPIWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Genre> Genres { get; set; } = null!;

        public DbSet<Movie> Movies { get; set; } = null!;
    }
}
