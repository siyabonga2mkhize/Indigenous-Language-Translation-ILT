using InnoDevsITL.Data;
using InnoDevsITL.Models;
using InnoDevsITL.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InnoDevsITL.Services.Implementations
{
    public class FavouriteService : IFavouriteService
    {
        private readonly InnoDbContext _context;

        public FavouriteService(InnoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Favourite>> GetFavouritesByUserAsync(string userId)
        {
            return await _context.Favourites
                .Include(f => f.Phrase)
                .Include(f => f.Phrase.Category)
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }

        public async Task<Favourite> AddFavouriteAsync(Favourite favourite)
        {
            _context.Favourites.Add(favourite);
            await _context.SaveChangesAsync();
            return favourite;
        }

        public async Task<bool> RemoveFavouriteAsync(int id)
        {
            var favourite = await _context.Favourites.FindAsync(id);
            if (favourite == null)
                return false;

            _context.Favourites.Remove(favourite);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsFavouriteAsync(string userId, int phraseId)
        {
            return await _context.Favourites
                .AnyAsync(f => f.UserId == userId && f.PhraseId == phraseId);
        }
    }
}
