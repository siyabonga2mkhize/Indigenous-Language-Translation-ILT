using InnoDevsITL.Data;
using InnoDevsITL.Models;
using InnoDevsITL.Services.Interfaces;
using InnoDevsITL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // IMPORTANT: Add this!

namespace InnoDevsITL.Controllers
{
    [Authorize]
    public class SuggestionController : Controller
    {
        private readonly InnoDbContext _context;
        private readonly UserManager<Users> _userManager;
        private readonly ITranslationService _translationService;

        public SuggestionController(
            InnoDbContext context,
            UserManager<Users> userManager,
            ITranslationService translationService)
        {
            _context = context;
            _userManager = userManager;
            _translationService = translationService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var submissions = await _context.Submissions
                .Include(s => s.Phrase)
                .Include(s => s.Phrase.Category)
                .Where(s => s.UserId == user.Id)
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();

            return View(submissions);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Languages = await _translationService.GetSupportedLanguagesAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(SuggestionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Create the phrase
                var phrase = new Phrase
                {
                    EnglishText = model.EnglishText,
                    Language = model.TargetLanguage,
                    CategoryId = model.CategoryId,
                    IsActive = false
                };

                _context.Phrases.Add(phrase);
                await _context.SaveChangesAsync();

                // Create the submission
                var submission = new Submission
                {
                    UserId = user.Id,
                    SubmittedText = model.TranslationText,
                    PhraseId = phrase.Id,
                    IsApproved = false,
                    SubmittedAt = DateTime.UtcNow
                };

                _context.Submissions.Add(submission);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Your suggestion has been submitted for review!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Languages = await _translationService.GetSupportedLanguagesAsync();
            return View(model);
        }
    }
}