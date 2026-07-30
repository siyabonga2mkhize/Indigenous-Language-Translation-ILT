using PhraseBookk.Models;

namespace PhraseBookk.Services
{
    public interface IAiTranslationService
    {
        Task<string> GenerateTranslationAsync(string englishText, LanguageCode targetLanguage);
        Task<string> GeneratePhraseInActionAsync(string englishText, string categoryName);
    }
}