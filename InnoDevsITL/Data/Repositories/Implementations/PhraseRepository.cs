using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InnoDevsITL.Data.Repositories.Interfaces;
using InnoDevsITL.Models;
using Microsoft.EntityFrameworkCore;

namespace InnoDevsITL.Data.Repositories.Implementations
{
    public class PhraseRepository : IPhraseRepository
    {
        private readonly InnoDbContext _context;

        public PhraseRepository(InnoDbContext context)
        {
            _context = context;
        }

        public async Task<Phrase> AddAsync(Phrase phrase)
        {
            await _context.Phrases.AddAsync(phrase);
            await _context.SaveChangesAsync();
            return phrase;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var phrase = await _context.Phrases.FindAsync(id);
            if (phrase == null)
            {
                return false;
            }

            _context.Phrases.Remove(phrase);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Phrase> GetByIdAsync(int id)
        {
            return await _context.Phrases
                .Include(p => p.Category)
                .Include(p => p.Translations)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Phrase>> SearchAsync(string searchTerm, int? categoryId)
        {
            var query = _context.Phrases
                .Include(p => p.Category)
                .Include(p => p.Translations)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.EnglishText.Contains(searchTerm) || p.Language.Contains(searchTerm));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<Phrase> UpdateAsync(Phrase phrase)
        {
            _context.Phrases.Update(phrase);
            await _context.SaveChangesAsync();
            return phrase;
        }
    }
}
