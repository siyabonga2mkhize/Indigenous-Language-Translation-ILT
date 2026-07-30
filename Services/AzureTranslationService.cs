using System.Text;
using System.Text.Json;
using PhraseBookk.Models;

namespace PhraseBookk.Services
{
    public class AzureTranslationService : IAiTranslationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _endpoint;
        private readonly string _region;

        public AzureTranslationService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            // ✅ READ FROM CONFIGURATION (NOT HARDCODED)
            _apiKey = configuration["AzureTranslator:ApiKey"] ?? "";
            _endpoint = configuration["AzureTranslator:Endpoint"] ?? "https://api.cognitive.microsofttranslator.com";
            _region = configuration["AzureTranslator:Region"] ?? "southafricanorth";

            Console.WriteLine($"=== Azure Translation Service Initialized ===");
            Console.WriteLine($"Endpoint: {_endpoint}");
            Console.WriteLine($"Region: {_region}");
            Console.WriteLine($"API Key (first 5 chars): {_apiKey?.Substring(0, Math.Min(5, _apiKey.Length))}...");
        }

        public async Task<string> GenerateTranslationAsync(string englishText, LanguageCode targetLanguage)
        {
            Console.WriteLine($"=== Azure Translation Called ===");
            Console.WriteLine($"Text: '{englishText}'");
            Console.WriteLine($"Target Language: {targetLanguage}");

            try
            {
                var targetLang = GetLanguageCode(targetLanguage);
                var url = $"{_endpoint}/translate?api-version=3.0&to={targetLang}";

                Console.WriteLine($"URL: {url}");

                var requestBody = new object[]
                {
                    new { Text = englishText }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("Ocp-Apim-Subscription-Key", _apiKey);
                request.Headers.Add("Ocp-Apim-Subscription-Region", _region);
                request.Content = content;

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Azure Translator error: {error}");
                    return GetFallbackMessage(englishText, targetLanguage);
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"✅ Azure response: {responseJson}");

                using var doc = JsonDocument.Parse(responseJson);
                var root = doc.RootElement;

                var translation = root[0]
                    .GetProperty("translations")[0]
                    .GetProperty("text")
                    .GetString();

                Console.WriteLine($"✅ Translation: {translation}");
                return translation ?? GetFallbackMessage(englishText, targetLanguage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Azure Translator exception: {ex.Message}");
                return GetFallbackMessage(englishText, targetLanguage);
            }
        }

        public async Task<string> GeneratePhraseInActionAsync(string englishText, string categoryName)
        {
            return $"💡 When you need help with '{englishText}', visit the {categoryName} office on campus.";
        }

        private string GetLanguageCode(LanguageCode code)
        {
            return code switch
            {
                LanguageCode.en => "en",
                LanguageCode.af => "af",
                LanguageCode.zu => "zu",
                LanguageCode.xh => "xh",
                LanguageCode.st => "st",
                LanguageCode.nso => "nso",
                LanguageCode.tn => "tn",
                LanguageCode.ts => "ts",
                LanguageCode.ss => "ss",
                LanguageCode.ve => "ve",
                LanguageCode.nr => "nr",
                _ => code.ToString()
            };
        }

        private string GetLanguageDisplayName(LanguageCode code)
        {
            return code switch
            {
                LanguageCode.en => "English",
                LanguageCode.af => "Afrikaans",
                LanguageCode.zu => "isiZulu",
                LanguageCode.xh => "isiXhosa",
                LanguageCode.st => "Sesotho",
                LanguageCode.nso => "Sepedi",
                LanguageCode.tn => "Setswana",
                LanguageCode.ts => "Xitsonga",
                LanguageCode.ss => "siSwati",
                LanguageCode.ve => "Tshivenda",
                LanguageCode.nr => "isiNdebele",
                _ => code.ToString()
            };
        }

        private string GetFallbackMessage(string englishText, LanguageCode targetLanguage)
        {
            var langName = GetLanguageDisplayName(targetLanguage);
            return $"⚠️ AI translation temporarily unavailable for {langName}. Please type your own translation below.";
        }
    }
}