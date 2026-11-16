
using Microsoft.EntityFrameworkCore;
using MoviesAPIWeb.Data;
using MoviesAPIWeb.Models;

namespace MoviesAPIWeb.Services
{
    public class GenresService : IGenresService
    {
        private readonly ApplicationDbContext _context;

        public GenresService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Genre>> GetAll()
        {
            return await _context.Genres
                .OrderBy(g => g.Name)
                .ToListAsync();
        }

        public async Task<Genre?> GetById(byte id)
        {
            return await _context.Genres
                .SingleOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Genre> Add(Genre genre)
        {
            await _context.Genres.AddAsync(genre);
            await _context.SaveChangesAsync();

            return genre;
        }

        public Genre Update(Genre genre)
        {
            _context.Genres.Update(genre);
            _context.SaveChanges();

            return genre;
        }

        public Genre Delete(Genre genre)
        {
            _context.Genres.Remove(genre);
            _context.SaveChanges();

            return genre;
        }

        public async Task<bool> IsvalidGenre(byte id)
        {
            return await _context.Genres
                .AnyAsync(g => g.Id == id);
        }
    }
}
