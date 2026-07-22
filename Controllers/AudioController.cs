using Microsoft.AspNetCore.Mvc;
using PhraseBookk.Services;
using System;
using System.Threading.Tasks;

namespace PhraseBookk.Controllers
{
    public class AudioController : Controller
    {
        private readonly IAudioService _audioService;

        public AudioController(IAudioService audioService)
        {
            _audioService = audioService;
        }

        [HttpPost]
        public async Task<IActionResult> GetAudio(string text, string language)
        {
            try
            {
                var audioBytes = await _audioService.GenerateAudioAsync(text, language);
                return File(audioBytes, "audio/wav");
            }
            catch (Exception ex)
            {
                return BadRequest($"Audio generation failed: {ex.Message}");
            }
        }
    }
}