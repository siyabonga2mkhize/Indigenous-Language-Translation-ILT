using InnoDevsITL.Data;
using InnoDevsITL.Models;
using InnoDevsITL.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InnoDevsITL.Services.Implementations
{
    public class MockTranslationService : ITranslationService
    {
        private readonly InnoDbContext _context;

        public MockTranslationService(InnoDbContext context)
        {
            _context = context;
        }

        // Database methods
        public async Task<IEnumerable<Translation>> GetTranslationsByPhraseIdAsync(int phraseId)
        {
            return await _context.Translations
                .Where(t => t.PhraseId == phraseId)
                .ToListAsync();
        }

        public async Task<Translation> GetTranslationByIdAsync(int id)
        {
            return await _context.Translations
                .Include(t => t.Phrase)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Translation> CreateTranslationAsync(Translation translation)
        {
            _context.Translations.Add(translation);
            await _context.SaveChangesAsync();
            return translation;
        }

        public async Task<Translation> UpdateTranslationAsync(Translation translation)
        {
            _context.Translations.Update(translation);
            await _context.SaveChangesAsync();
            return translation;
        }

        public async Task<bool> DeleteTranslationAsync(int id)
        {
            var translation = await _context.Translations.FindAsync(id);
            if (translation == null)
                return false;

            _context.Translations.Remove(translation);
            await _context.SaveChangesAsync();
            return true;
        }

        // Translation API methods - MUST MATCH INTERFACE EXACTLY
        public async Task<string> TranslateTextAsync(string text, string targetLanguage, string sourceLanguage = "en")
        {
            await Task.Delay(100);
            return $"[{targetLanguage}] {text}";
        }

        public async Task<IEnumerable<string>> GetSupportedLanguagesAsync()
        {
            await Task.Delay(100);
            return new List<string> { "en", "es", "fr", "de", "zh", "ja", "ar" };
        }
    }
}