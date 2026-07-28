using InnoDevsITL.Data;
using InnoDevsITL.Models;
using InnoDevsITL.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InnoDevsITL.Services.Implementations
{
    public class PhraseService : IPhraseService
    {
        private readonly InnoDbContext _context;

        public PhraseService(InnoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Phrase>> SearchPhrasesAsync(string searchTerm, int? categoryId)
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

        public async Task<Phrase> GetPhraseByIdAsync(int id)
        {
            return await _context.Phrases
                .Include(p => p.Category)
                .Include(p => p.Translations)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Phrase> CreatePhraseAsync(Phrase phrase)
        {
            _context.Phrases.Add(phrase);
            await _context.SaveChangesAsync();
            return phrase;
        }

        public async Task<Phrase> UpdatePhraseAsync(Phrase phrase)
        {
            _context.Phrases.Update(phrase);
            await _context.SaveChangesAsync();
            return phrase;
        }

        public async Task<bool> DeletePhraseAsync(int id)
        {
            var phrase = await _context.Phrases.FindAsync(id);
            if (phrase == null)
                return false;

            _context.Phrases.Remove(phrase);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Phrase>> GetPhrasesByCategoryAsync(int categoryId)
        {
            return await _context.Phrases
                .Include(p => p.Translations)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }
    }
}