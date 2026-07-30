using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhraseBookk.Data;
using PhraseBookk.Models;
using PhraseBookk.Services;

namespace PhraseBookk.Controllers
{
    public class SurvivalController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAudioService _audioService;

        public SurvivalController(ApplicationDbContext context, IAudioService audioService)
        {
            _context = context;
            _audioService = audioService;
        }

        // GET: Survival/Index
        public async Task<IActionResult> Index()
        {
            var survivalPhrases = await _context.Phrases
                .Where(p => p.IsSurvival && p.IsActive)
                .Include(p => p.Category)
                .Include(p => p.Translations)
                .OrderBy(p => p.Category.Name)
                .ThenBy(p => p.EnglishText)
                .ToListAsync();

            ViewBag.TotalSurvival = survivalPhrases.Count;
            return View(survivalPhrases);
        }

        // GET: Survival/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var phrase = await _context.Phrases
                .Include(p => p.Category)
                .Include(p => p.Translations)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsSurvival && p.IsActive);

            if (phrase == null)
            {
                return NotFound();
            }

            return View(phrase);
        }

        // ✅ NEW: GET: Survival/PlayAudio/5
        [HttpGet]
        public async Task<IActionResult> PlayAudio(int id)
        {
            var translation = await _context.Translations
                .Include(t => t.Phrase)
                .FirstOrDefaultAsync(t => t.Id == id && t.Status == ContentStatus.Approved);

            if (translation == null)
            {
                return NotFound();
            }

            try
            {
                // Generate audio using the Azure TTS service
                var audioBytes = await _audioService.GenerateAudioAsync(
                    translation.TranslatedText,
                    translation.Language.ToString()
                );

                // Return as MP3 file
                return File(audioBytes, "audio/mpeg", $"{translation.Id}.mp3");
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Audio generation failed: {ex.Message}");
                return StatusCode(500, "Audio generation failed");
            }
        }
    }
}