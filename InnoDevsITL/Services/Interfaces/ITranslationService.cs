using InnoDevsITL.Models;

namespace InnoDevsITL.Services.Interfaces  // This should be the namespace
{
    public interface ITranslationService
    {
        // Database operations
        Task<IEnumerable<Translation>> GetTranslationsByPhraseIdAsync(int phraseId);
        Task<Translation> GetTranslationByIdAsync(int id);
        Task<Translation> CreateTranslationAsync(Translation translation);
        Task<Translation> UpdateTranslationAsync(Translation translation);
        Task<bool> DeleteTranslationAsync(int id);

        // Translation API operations
        Task<string> TranslateTextAsync(string text, string targetLanguage, string sourceLanguage = "en");
        Task<IEnumerable<string>> GetSupportedLanguagesAsync();
    }
}