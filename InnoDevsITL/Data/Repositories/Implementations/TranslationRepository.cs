using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InnoDevsITL.Data.Repositories.Interfaces;
using InnoDevsITL.Models;
using Microsoft.EntityFrameworkCore;

namespace InnoDevsITL.Data.Repositories.Implementations
{
    public class TranslationRepository : ITranslationRepository
    {
        private readonly InnoDbContext _context;

        public TranslationRepository(InnoDbContext context)
        {
            _context = context;
        }

        public async Task<Translation> AddAsync(Translation translation)
        {
            await _context.Translations.AddAsync(translation);
            await _context.SaveChangesAsync();
            return translation;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var translation = await _context.Translations.FindAsync(id);
            if (translation == null)
            {
                return false;
            }

            _context.Translations.Remove(translation);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Translation> GetByIdAsync(int id)
        {
            return await _context.Translations
                .Include(t => t.Phrase)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Translation>> GetByPhraseIdAsync(int phraseId)
        {
            return await _context.Translations
                .Where(t => t.PhraseId == phraseId)
                .ToListAsync();
        }

        public async Task<Translation> UpdateAsync(Translation translation)
        {
            _context.Translations.Update(translation);
            await _context.SaveChangesAsync();
            return translation;
        }
    }
}
