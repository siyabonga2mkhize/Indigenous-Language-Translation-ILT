using InnoDevsITL.Data;
using InnoDevsITL.Models;
using InnoDevsITL.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace InnoDevsITL.Services.Implementations
{
    public class TranslationService : ITranslationService
    {
        private readonly InnoDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public TranslationService(InnoDbContext context, IConfiguration config, HttpClient httpClient)
        {
            _context = context;
            _config = config;
            _httpClient = httpClient;
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
            try
            {
                var endpoint = _config["AzureAI:Translator:Endpoint"] ?? "https://api.cognitive.microsofttranslator.com/";
                var key = _config["AzureAI:Translator:Key"] ?? "";
                var region = _config["AzureAI:Translator:Region"] ?? "global";

                if (string.IsNullOrEmpty(key))
                {
                    return $"[{targetLanguage}] {text}";
                }

                var url = $"{endpoint}translate?api-version=3.0&from={sourceLanguage}&to={targetLanguage}";

                var requestBody = new object[] { new { Text = text } };
                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
                _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", region);

                var response = await _httpClient.PostAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(responseContent);
                    var translation = doc.RootElement[0]
                        .GetProperty("translations")[0]
                        .GetProperty("text")
                        .GetString();

                    return translation ?? text;
                }

                return text;
            }
            catch
            {
                return text;
            }
        }

        public async Task<IEnumerable<string>> GetSupportedLanguagesAsync()
        {
            try
            {
                var endpoint = _config["AzureAI:Translator:Endpoint"] ?? "https://api.cognitive.microsofttranslator.com/";
                var key = _config["AzureAI:Translator:Key"] ?? "";

                if (string.IsNullOrEmpty(key))
                {
                    return new List<string> { "en", "es", "fr", "de", "zh", "ja", "ar" };
                }

                var url = $"{endpoint}languages?api-version=3.0";

                _httpClient.DefaultRequestHeaders.Clear();

                var response = await _httpClient.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(responseContent);
                    var languages = doc.RootElement
                        .GetProperty("translation")
                        .EnumerateObject()
                        .Select(p => p.Name)
                        .ToList();

                    return languages;
                }

                return new List<string> { "en", "es", "fr", "de", "zh", "ja", "ar" };
            }
            catch
            {
                return new List<string> { "en", "es", "fr", "de", "zh", "ja", "ar" };
            }
        }
    }
}