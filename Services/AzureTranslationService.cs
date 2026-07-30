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
            _apiKey = configuration["AzureTranslator:ApiKey"] ?? throw new Exception("Azure Translator API key not found");
            _endpoint = configuration["AzureTranslator:Endpoint"] ?? "https://api.cognitive.microsofttranslator.com";
            _region = configuration["AzureTranslator:Region"] ?? "southafricanorth";
        }

        public async Task<string> GenerateTranslationAsync(string englishText, LanguageCode targetLanguage)
        {
            if (string.IsNullOrEmpty(_apiKey) || _apiKey == "YOUR_AZURE_TRANSLATOR_KEY_HERE")
            {
                return $"⚠️ Azure Translator unavailable. Please add your API key to appsettings.json. Suggested translation: [AI would translate: {englishText}]";
            }

            try
            {
                var targetLang = GetLanguageCode(targetLanguage);
                var url = $"{_endpoint}/translate?api-version=3.0&to={targetLang}";

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
                    throw new Exception($"Azure Translator error: {response.StatusCode} - {error}");
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseJson);
                var root = doc.RootElement;

                var translation = root[0]
                    .GetProperty("translations")[0]
                    .GetProperty("text")
                    .GetString();

                return translation ?? string.Empty;
            }
            catch (Exception ex)
            {
                return $" Translation temporarily unavailable. Error: {ex.Message}";
            }
        }

        public async Task<string> GeneratePhraseInActionAsync(string englishText, string categoryName)
        {
            return $" When you need help with '{englishText}', visit the {categoryName} office on campus.";
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
    }
}