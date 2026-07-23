using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace PhraseBookk.Services
{
    public class AzureTtsService : IAudioService
    {
        private readonly string _subscriptionKey;
        private readonly string _region;

        public AzureTtsService(IConfiguration configuration)
        {
            _subscriptionKey = configuration["AzureSpeech:Key"];
            _region = configuration["AzureSpeech:Region"];
        }

        public async Task<byte[]> GenerateAudioAsync(string text, string languageCode)
        {
            // Create a speech config with your key and region
            var config = SpeechConfig.FromSubscription(_subscriptionKey, _region);

            // Set the language (e.g., "zu-ZA", "en-US")
            config.SpeechSynthesisLanguage = languageCode;

            // Use the default speaker (neural voice)
            using var synthesizer = new SpeechSynthesizer(config);
            var result = await synthesizer.SpeakTextAsync(text);

            if (result.Reason == ResultReason.SynthesizingAudioCompleted)
            {
                return result.AudioData;
            }
            else
            {
                throw new Exception($"Speech synthesis failed: {result.Reason}");
            }
        }
    }
}