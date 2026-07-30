using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhraseBookk.Data;
using PhraseBookk.Models;
using PhraseBookk.ViewModels;
using PhraseBookk.Services;

namespace PhraseBookk.Controllers
{
    public class PhrasesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PhrasesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Phrases/Index
        public async Task<IActionResult> Index(string? keyword, int? categoryId)
        {
            var query = _context.Phrases
                .Include(p => p.Category)
                .Include(p => p.Translations)
                .Where(p => p.IsActive);

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var searchTerm = keyword.ToLower();
                query = query.Where(p =>
                    p.EnglishText.ToLower().Contains(searchTerm) ||
                    (p.Translations != null && p.Translations.Any(t => t.TranslatedText.ToLower().Contains(searchTerm) && t.Status == ContentStatus.Approved))
                );
            }

            var phrases = await query.OrderBy(p => p.EnglishText).ToListAsync();
            var categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();

            var viewModel = new PhraseSearchViewModel
            {
                SearchKeyword = keyword,
                CategoryId = categoryId,
                Categories = categories,
                Results = phrases.Select(p => new PhraseResultViewModel
                {
                    PhraseId = p.Id,
                    EnglishText = p.EnglishText,
                    CategoryName = p.Category?.Name ?? "Uncategorized",
                    CategoryId = p.CategoryId,
                    IsFavorited = false,
                    MatchedTranslations = p.Translations?
                        .Where(t => t.Status == ContentStatus.Approved &&
                                   (string.IsNullOrWhiteSpace(keyword) ||
                                    t.TranslatedText.ToLower().Contains(keyword.ToLower())))
                        .Select(t => new MatchedTranslation
                        {
                            LanguageName = t.Language.ToString(),
                            TranslatedText = t.TranslatedText,
                            HighlightedText = HighlightMatch(t.TranslatedText, keyword)
                        })
                        .ToList() ?? new List<MatchedTranslation>()
                }).ToList()
            };

            // ✅ Save search to history
            if (!string.IsNullOrWhiteSpace(keyword) && User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var stat = new UsageStat
                    {
                        UserId = user.Id,
                        SearchKeyword = keyword,
                        LanguageSelected = "en",
                        Timestamp = DateTime.Now,
                        Action = "Search"
                    };
                    _context.UsageStats.Add(stat);
                    await _context.SaveChangesAsync();
                }
            }

            return View(viewModel);
        }

        // Helper method to highlight matching text
        private string HighlightMatch(string text, string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword) || string.IsNullOrWhiteSpace(text))
                return text;

            var index = text.IndexOf(keyword, StringComparison.OrdinalIgnoreCase);
            if (index < 0) return text;

            return $"{text.Substring(0, index)}<mark>{text.Substring(index, keyword.Length)}</mark>{text.Substring(index + keyword.Length)}";
        }

        // GET: Phrases/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var phrase = await _context.Phrases
                .Include(p => p.Category)
                .Include(p => p.Translations)!
                    .ThenInclude(t => t.Votes)
                .Include(p => p.Translations)!
                    .ThenInclude(t => t.SubmittedBy)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (phrase == null)
            {
                return NotFound();
            }

            // ✅ Save view to history
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var stat = new UsageStat
                    {
                        UserId = user.Id,
                        PhraseId = phrase.Id,
                        Category = phrase.Category?.Name,
                        Timestamp = DateTime.Now,
                        Action = "View"
                    };
                    _context.UsageStats.Add(stat);
                    await _context.SaveChangesAsync();
                }
            }

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userId != null)
                {
                    ViewBag.IsFavourited = await _context.Favorites
                        .AnyAsync(f => f.PhraseId == id && f.UserId == userId);
                }
            }

            return View(phrase);
        }

        // GET: Phrases/SubmitTranslation/5
        [Authorize]
        public async Task<IActionResult> SubmitTranslation(int id)
        {
            var phrase = await _context.Phrases
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (phrase == null)
            {
                return NotFound();
            }

            var model = new TranslationSubmissionViewModel
            {
                PhraseId = phrase.Id,
                PhraseEnglishText = phrase.EnglishText,
                PhraseCategoryName = phrase.Category?.Name,
                AvailableLanguages = Enum.GetValues(typeof(LanguageCode))
                    .Cast<LanguageCode>()
                    .Select(l => new { Value = l, Name = l.ToString() })
                    .ToList<dynamic>()
            };

            return View(model);
        }

        // POST: Phrases/SubmitTranslation
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitTranslation(TranslationSubmissionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized();
                }

                // ✅ Check if user is a Trusted Contributor (10+ approved translations)
                var autoApproveThreshold = 10;
                var isTrustedContributor = IsTrustedContributor(user);

                var translation = new Translation
                {
                    PhraseId = model.PhraseId,
                    Language = model.Language,
                    TranslatedText = model.TranslatedText,
                    Status = isTrustedContributor ? ContentStatus.Approved : ContentStatus.Pending,
                    CreatedDate = DateTime.Now,
                    SubmittedById = user.Id,
                    ReviewedDate = isTrustedContributor ? DateTime.Now : null
                };

                _context.Translations.Add(translation);
                await _context.SaveChangesAsync();

                // ✅ If auto-approved, increment their count
                if (isTrustedContributor)
                {
                    user.TotalApprovedTranslations += 1;
                    await _userManager.UpdateAsync(user);

                    TempData["SuccessMessage"] = "🎉 Translation auto-approved! You're a Trusted Contributor!";
                }
                else
                {
                    var remaining = autoApproveThreshold - user.TotalApprovedTranslations;
                    TempData["SuccessMessage"] = $"Translation submitted! You need {remaining} more approved translations to become a Trusted Contributor and get auto-approval.";
                }

                return RedirectToAction(nameof(Details), new { id = model.PhraseId });
            }

            var phrase = await _context.Phrases
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == model.PhraseId);

            if (phrase != null)
            {
                model.PhraseEnglishText = phrase.EnglishText;
                model.PhraseCategoryName = phrase.Category?.Name;
            }

            model.AvailableLanguages = Enum.GetValues(typeof(LanguageCode))
                .Cast<LanguageCode>()
                .Select(l => new { Value = l, Name = l.ToString() })
                .ToList<dynamic>();

            return View(model);
        }

        // ✅ Helper method to check if user is a Trusted Contributor
        private bool IsTrustedContributor(ApplicationUser user)
        {
            return user.TotalApprovedTranslations >= 10;
        }

        // ✅ DTO for deserializing JSON requests
        public class AiTranslationRequest
        {
            public int PhraseId { get; set; }
            public string Language { get; set; } = string.Empty;
        }

        // ✅ AJAX: Get AI Translation Draft - FIXED
        [HttpPost]
        public async Task<IActionResult> GetAiTranslation([FromBody] AiTranslationRequest request)
        {
            if (request == null)
            {
                return Json(new { success = false, message = "Invalid request payload." });
            }

            // ✅ Log what we received for debugging
            Console.WriteLine($"Received - phraseId: {request.PhraseId}, language: '{request.Language}'");

            // ✅ Check if language is empty
            if (string.IsNullOrEmpty(request.Language))
            {
                return Json(new { success = false, message = "Please select a language first." });
            }

            // ✅ Parse the language from string to LanguageCode enum
            if (!Enum.TryParse<LanguageCode>(request.Language, true, out var languageCode))
            {
                return Json(new { success = false, message = $"Invalid language code: '{request.Language}'. Please select a valid language." });
            }

            var phrase = await _context.Phrases.FindAsync(request.PhraseId);
            if (phrase == null)
            {
                return Json(new { success = false, message = $"Phrase not found (ID: {request.PhraseId})" });
            }

            try
            {
                var aiService = HttpContext.RequestServices.GetRequiredService<IAiTranslationService>();
                var translation = await aiService.GenerateTranslationAsync(phrase.EnglishText, languageCode);
                return Json(new { success = true, translation });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Phrases/MySubmissions
        [Authorize]
        public async Task<IActionResult> MySubmissions(string? statusFilter)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var query = _context.Translations
                .Include(t => t.Phrase)!
                    .ThenInclude(p => p.Category)
                .Where(t => t.SubmittedById == user.Id);

            // ✅ Apply status filter
            if (!string.IsNullOrEmpty(statusFilter))
            {
                var statusEnum = Enum.Parse<ContentStatus>(statusFilter);
                query = query.Where(t => t.Status == statusEnum);
            }

            var translations = await query
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();

            // ✅ Count by status for ViewBag
            var allTranslations = await _context.Translations
                .Where(t => t.SubmittedById == user.Id)
                .ToListAsync();

            ViewBag.PendingCount = allTranslations.Count(t => t.Status == ContentStatus.Pending);
            ViewBag.ApprovedCount = allTranslations.Count(t => t.Status == ContentStatus.Approved);
            ViewBag.RejectedCount = allTranslations.Count(t => t.Status == ContentStatus.Rejected);
            ViewBag.TotalCount = allTranslations.Count;
            ViewBag.CurrentFilter = statusFilter;

            return View(translations);
        }

        // GET: Phrases/History - SHOW LAST 30 MINUTES ONLY
        [Authorize]
        public async Task<IActionResult> History()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var thirtyMinutesAgo = DateTime.Now.AddMinutes(-30);

            var history = await _context.UsageStats
                .Where(u => u.UserId == user.Id && u.Timestamp >= thirtyMinutesAgo)
                .Include(u => u.Phrase)
                .OrderByDescending(u => u.Timestamp)
                .ToListAsync();

            return View(history);
        }
    }
}