using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InnoDevsITL.Data.Repositories.Interfaces;
using InnoDevsITL.Models;
using Microsoft.EntityFrameworkCore;

namespace InnoDevsITL.Data.Repositories.Implementations
{
    public class FavouriteRepository : IFavouriteRepository
    {
        private readonly InnoDbContext _context;

        public FavouriteRepository(InnoDbContext context)
        {
            _context = context;
        }

        public async Task<Favourite> AddAsync(Favourite favourite)
        {
            await _context.Favourites.AddAsync(favourite);
            await _context.SaveChangesAsync();
            return favourite;
        }

        public async Task<IEnumerable<Favourite>> GetByUserAsync(string userId)
        {
            return await _context.Favourites
                .Include(f => f.Phrase)
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var favourite = await _context.Favourites.FindAsync(id);
            if (favourite == null)
            {
                return false;
            }

            _context.Favourites.Remove(favourite);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
