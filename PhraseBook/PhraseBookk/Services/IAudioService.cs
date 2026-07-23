using System.Threading.Tasks;

namespace PhraseBookk.Services
{
    public interface IAudioService
    {
        Task<byte[]> GenerateAudioAsync(string text, string languageCode);
    }
}