using System.Text;
using System.Text.Json;
using PhraseBookk.Models;

namespace PhraseBookk.Services
{
    public class GeminiAiService : IAiTranslationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;

        public GeminiAiService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Gemini:ApiKey"] ?? throw new Exception("Gemini API key not found");
            // ✅ FIXED: Use gemini-2.0-flash (the correct model name)
            _model = configuration["Gemini:Model"] ?? "gemini-2.0-flash";
        }

        public async Task<string> GenerateTranslationAsync(string englishText, LanguageCode targetLanguage)
        {
            if (string.IsNullOrEmpty(_apiKey) || _apiKey == "YOUR_GEMINI_API_KEY_HERE")
            {
                return $"⚠️ AI translation unavailable. Please add your Gemini API key to appsettings.json. Suggested translation: [AI would translate: {englishText}]";
            }

            try
            {
                var languageName = GetLanguageName(targetLanguage);
                var prompt = $@"
You are a translator for a South African university campus phrasebook called PhraseBook.

Translate the following English phrase into {languageName}.
The phrase is: '{englishText}'

Return ONLY the translation, nothing else. No explanations, no quotes, just the translated text.
The translation should be natural and appropriate for a university campus context.
If the phrase is a question, translate it as a question.
If the phrase is a statement, translate it as a statement.
Keep the translation concise and clear.
";

                var response = await SendGeminiRequestAsync(prompt);
                return response.Trim();
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("429") || ex.Message.Contains("TooManyRequests"))
            {
                return "⚠️ AI rate limit reached. Free tier requests are temporarily throttled — please wait a minute and try again!";
            }
            catch (Exception ex)
            {
                return $"⚠️ AI translation temporarily unavailable. Error: {ex.Message}";
            }
        }

        public async Task<string> GeneratePhraseInActionAsync(string englishText, string categoryName)
        {
            if (string.IsNullOrEmpty(_apiKey) || _apiKey == "YOUR_GEMINI_API_KEY_HERE")
            {
                return $"💡 Tip: When you ask '{englishText}', make sure to visit the {categoryName} office during working hours.";
            }

            try
            {
                var prompt = $@"
You are a campus guide for DUT (Durban University of Technology).

For the phrase: '{englishText}' (Category: {categoryName})

Write a short, practical tip (2-3 sentences) that helps a student know what to do when they need to use this phrase on campus.

Example format:
- For 'Where is the registration office?': 'Registration takes place at Ritson Hall. Arrive early as queues can be 2+ hours. Bring your ID and proof of acceptance.'

Return ONLY the tip, nothing else. No quotes, no explanations.
Make it practical and specific to DUT campus life.
";

                var response = await SendGeminiRequestAsync(prompt);
                return response.Trim();
            }
            catch (Exception)
            {
                return $"💡 Tip: When you need help with '{englishText}', visit the {categoryName} office on campus.";
            }
        }

        private async Task<string> SendGeminiRequestAsync(string prompt)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = 0.7,
                    maxOutputTokens = 256
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Gemini API error: {response.StatusCode} - {error}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);
            var root = doc.RootElement;

            var text = root
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return text ?? string.Empty;
        }

        private static string GetLanguageName(LanguageCode code)
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
    }
}