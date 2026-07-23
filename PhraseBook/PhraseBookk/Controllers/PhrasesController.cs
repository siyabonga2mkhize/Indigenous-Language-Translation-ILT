using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhraseBookk.Data;
using PhraseBookk.Models;
using PhraseBookk.ViewModels;
using System.Text.Json;

namespace PhraseBookk.Controllers
{
    public class PhrasesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PhrasesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Phrases/Index
        public async Task<IActionResult> Index(string? keyword, int? categoryId)
        {
            var viewModel = new PhraseSearchViewModel
            {
                SearchKeyword = keyword,
                CategoryId = categoryId,
                Categories = await _context.Categories.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync(),
                Results = new List<PhraseResultViewModel>()
            };

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var translationMatches = await _context.Translations
                    .Where(t => t.Status == ContentStatus.Approved && t.TranslatedText.ToLower().Contains(keyword.ToLower()))
                    .Select(t => t.PhraseId)
                    .Distinct()
                    .ToListAsync();

                var englishMatches = await _context.Phrases
                    .Where(p => p.IsActive && p.EnglishText.ToLower().Contains(keyword.ToLower()))
                    .Select(p => p.Id)
                    .ToListAsync();

                var allMatchedIds = translationMatches.Union(englishMatches).Distinct().ToList();

                if (categoryId.HasValue && categoryId.Value > 0)
                {
                    var categoryPhraseIds = await _context.Phrases
                        .Where(p => p.CategoryId == categoryId.Value && p.IsActive)
                        .Select(p => p.Id)
                        .ToListAsync();
                    allMatchedIds = allMatchedIds.Intersect(categoryPhraseIds).ToList();
                }

                var phrases = await _context.Phrases
                    .Include(p => p.Category)
                    .Include(p => p.Translations)
                    .Where(p => allMatchedIds.Contains(p.Id) && p.IsActive)
                    .ToListAsync();

                foreach (var phrase in phrases)
                {
                    var result = new PhraseResultViewModel
                    {
                        PhraseId = phrase.Id,
                        EnglishText = phrase.EnglishText,
                        CategoryName = phrase.Category?.Name ?? "Uncategorized",
                        CategoryId = phrase.CategoryId,
                        MatchedTranslations = new List<MatchedTranslation>()
                    };

                    if (phrase.EnglishText.ToLower().Contains(keyword.ToLower()))
                    {
                        result.MatchedTranslations.Add(new MatchedTranslation
                        {
                            LanguageName = "English",
                            TranslatedText = phrase.EnglishText,
                            HighlightedText = HighlightText(phrase.EnglishText, keyword)
                        });
                    }

                    var approvedTranslations = phrase.Translations?.Where(t => t.Status == ContentStatus.Approved);
                    if (approvedTranslations != null)
                    {
                        foreach (var trans in approvedTranslations)
                        {
                            result.MatchedTranslations.Add(new MatchedTranslation
                            {
                                LanguageName = trans.Language.ToString(),
                                TranslatedText = trans.TranslatedText,
                                HighlightedText = trans.TranslatedText.ToLower().Contains(keyword.ToLower())
                                    ? HighlightText(trans.TranslatedText, keyword)
                                    : trans.TranslatedText
                            });
                        }
                    }

                    viewModel.Results.Add(result);
                }

                if (categoryId.HasValue && categoryId.Value > 0)
                {
                    var stat = new UsageStat
                    {
                        LanguageSelected = LanguageCode.en,
                        CategoryId = categoryId.Value,
                        SearchKeyword = keyword,
                        ViewedAt = DateTime.Now,
                        UserId = User.Identity?.IsAuthenticated == true
                            ? _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name)?.Id
                            : null
                    };
                    _context.UsageStats.Add(stat);
                    await _context.SaveChangesAsync();
                }
            }

            return View(viewModel);
        }

        private string HighlightText(string text, string keyword)
        {
            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(keyword))
                return text;

            int index = text.IndexOf(keyword, StringComparison.OrdinalIgnoreCase);
            if (index < 0)
                return text;

            return text.Substring(0, index) +
                   "<mark>" + text.Substring(index, keyword.Length) + "</mark>" +
                   text.Substring(index + keyword.Length);
        }

        // ==================== AUTOCOMPLETE ====================
        public async Task<IActionResult> Autocomplete(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Json(new List<string>());
            }

            var phrases = await _context.Phrases
                .Where(p => p.IsActive && p.EnglishText.ToLower().Contains(term.ToLower()))
                .Select(p => p.EnglishText)
                .Take(10)
                .ToListAsync();

            var translations = await _context.Translations
                .Where(t => t.Status == ContentStatus.Approved && t.TranslatedText.ToLower().Contains(term.ToLower()))
                .Select(t => t.Phrase.EnglishText)
                .Take(10)
                .ToListAsync();

            var results = phrases.Union(translations).Distinct().Take(10).ToList();
            return Json(results);
        }

        // ==================== DETAILS (with History tracking) ====================
        public async Task<IActionResult> Details(int id)
        {
            var phrase = await _context.Phrases
                .Include(p => p.Category)
                .Include(p => p.Translations)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (phrase == null)
            {
                return NotFound();
            }

            // Log view statistics
            var stat = new UsageStat
            {
                LanguageSelected = LanguageCode.en,
                CategoryId = phrase.CategoryId,
                ViewedAt = DateTime.Now,
                UserId = User.Identity?.IsAuthenticated == true
                    ? _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name)?.Id
                    : null
            };
            _context.UsageStats.Add(stat);
            await _context.SaveChangesAsync();

            // Track recent views (for student history)
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name)?.Id;
                if (!string.IsNullOrEmpty(userId))
                {
                    var recentList = HttpContext.Session.GetString("RecentViews");
                    var recentIds = string.IsNullOrEmpty(recentList)
                        ? new List<int>()
                        : JsonSerializer.Deserialize<List<int>>(recentList);
                    if (recentIds != null)
                    {
                        recentIds.Remove(id);
                        recentIds.Insert(0, id);
                        if (recentIds.Count > 10)
                            recentIds.RemoveAt(recentIds.Count - 1);
                        HttpContext.Session.SetString("RecentViews", JsonSerializer.Serialize(recentIds));
                    }
                }
            }

            // Check if phrase is favourited
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name)?.Id;
                if (!string.IsNullOrEmpty(userId))
                {
                    ViewBag.IsFavourited = await _context.Favorites
                        .AnyAsync(f => f.UserId == userId && f.PhraseId == id);
                }
            }

            return View(phrase);
        }

        // ==================== HISTORY (Recent Views) ====================
        [Authorize]
        public async Task<IActionResult> History()
        {
            var recentList = HttpContext.Session.GetString("RecentViews");
            var recentIds = string.IsNullOrEmpty(recentList)
                ? new List<int>()
                : JsonSerializer.Deserialize<List<int>>(recentList);
            var phrases = new List<Phrase>();
            if (recentIds != null && recentIds.Any())
            {
                phrases = await _context.Phrases
                    .Include(p => p.Category)
                    .Include(p => p.Translations)
                    .Where(p => recentIds.Contains(p.Id))
                    .ToListAsync();
            }
            return View(phrases);
        }

        // ==================== SUBMIT TRANSLATION ====================
        [Authorize]
        public async Task<IActionResult> SubmitTranslation(int id)
        {
            var phrase = await _context.Phrases
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (phrase == null)
            {
                return NotFound();
            }

            var model = new TranslationSubmissionViewModel
            {
                PhraseId = phrase.Id,
                PhraseEnglishText = phrase.EnglishText,
                PhraseCategoryName = phrase.Category?.Name
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitTranslation(TranslationSubmissionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var phrase = await _context.Phrases
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == model.PhraseId && p.IsActive);
                if (phrase != null)
                {
                    model.PhraseEnglishText = phrase.EnglishText;
                    model.PhraseCategoryName = phrase.Category?.Name;
                }
                return View(model);
            }

            var existing = await _context.Translations
                .FirstOrDefaultAsync(t => t.PhraseId == model.PhraseId && t.Language == model.Language);

            if (existing != null)
            {
                ModelState.AddModelError("", $"A translation for {model.Language} already exists.");
                var phrase = await _context.Phrases
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == model.PhraseId && p.IsActive);
                if (phrase != null)
                {
                    model.PhraseEnglishText = phrase.EnglishText;
                    model.PhraseCategoryName = phrase.Category?.Name;
                }
                return View(model);
            }

            var userId = _context.Users.FirstOrDefault(u => u.UserName == User.Identity!.Name)?.Id;
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "Unable to identify user.");
                return View(model);
            }

            var translation = new Translation
            {
                PhraseId = model.PhraseId,
                Language = model.Language,
                TranslatedText = model.TranslatedText,
                Status = ContentStatus.Pending,
                SubmittedById = userId,
                CreatedDate = DateTime.Now
            };

            _context.Translations.Add(translation);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your translation suggestion has been submitted for review!";
            return RedirectToAction("Details", new { id = model.PhraseId });
        }

        [Authorize]
        public async Task<IActionResult> MySubmissions(string? statusFilter)
        {
            var userId = _context.Users.FirstOrDefault(u => u.UserName == User.Identity!.Name)?.Id;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var query = _context.Translations
                .Include(t => t.Phrase)
                .ThenInclude(p => p.Category)
                .Where(t => t.SubmittedById == userId);

            if (!string.IsNullOrEmpty(statusFilter) && Enum.TryParse<ContentStatus>(statusFilter, out var status))
            {
                query = query.Where(t => t.Status == status);
            }

            query = query.OrderByDescending(t => t.CreatedDate);

            var submissions = await query.ToListAsync();

            ViewBag.PendingCount = await _context.Translations.CountAsync(t => t.SubmittedById == userId && t.Status == ContentStatus.Pending);
            ViewBag.ApprovedCount = await _context.Translations.CountAsync(t => t.SubmittedById == userId && t.Status == ContentStatus.Approved);
            ViewBag.RejectedCount = await _context.Translations.CountAsync(t => t.SubmittedById == userId && t.Status == ContentStatus.Rejected);
            ViewBag.TotalCount = submissions.Count;
            ViewBag.CurrentFilter = statusFilter;

            return View(submissions);
        }
    }
}