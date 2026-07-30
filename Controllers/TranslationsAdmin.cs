using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhraseBookk.Data;
using PhraseBookk.Models;

namespace PhraseBookk.Controllers
{
    [Authorize]
    public class TranslationsAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TranslationsAdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: TranslationsAdmin/Pending
        public async Task<IActionResult> Pending()
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole("Administrator");
            var isModerator = user?.IsModerator == true;

            if (!isAdmin && !isModerator)
            {
                return RedirectToAction("Index", "Home");
            }

            var query = _context.Translations
                .Include(t => t.Phrase)
                .Include(t => t.SubmittedBy)
                .Where(t => t.Status == ContentStatus.Pending);

            if (!isAdmin && user != null && user.IsModerator && !string.IsNullOrEmpty(user.ModeratorLanguage))
            {
                var langEnum = Enum.Parse<LanguageCode>(user.ModeratorLanguage);
                query = query.Where(t => t.Language == langEnum);
            }

            var pendingTranslations = await query.ToListAsync();

            var topKeywords = await _context.UsageStats
                .Where(s => s.SearchKeyword != null)
                .GroupBy(s => s.SearchKeyword)
                .Select(g => new { Keyword = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .Take(20)
                .ToListAsync();

            foreach (var translation in pendingTranslations)
            {
                var phraseText = translation.Phrase?.EnglishText?.ToLower() ?? "";
                var priority = 0;

                foreach (var keyword in topKeywords)
                {
                    if (phraseText.Contains(keyword.Keyword.ToLower()))
                    {
                        priority += keyword.Count;
                    }
                }

                translation.AdminReviewComment = priority.ToString();
            }

            pendingTranslations = pendingTranslations
                .OrderByDescending(t => int.TryParse(t.AdminReviewComment, out var p) ? p : 0)
                .ToList();

            ViewBag.ApprovedCount = await _context.Translations.CountAsync(t => t.Status == ContentStatus.Approved);
            ViewBag.RejectedCount = await _context.Translations.CountAsync(t => t.Status == ContentStatus.Rejected);
            ViewBag.TotalTranslations = await _context.Translations.CountAsync();

            ViewBag.IsModerator = user?.IsModerator ?? false;
            ViewBag.ModeratorLanguage = user?.ModeratorLanguage ?? "All";

            return View(pendingTranslations);
        }

        // GET: TranslationsAdmin/Approve/5
        public async Task<IActionResult> Approve(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole("Administrator");
            var isModerator = user?.IsModerator == true;

            if (!isAdmin && !isModerator)
            {
                return RedirectToAction("Index", "Home");
            }

            var translation = await _context.Translations
                .Include(t => t.Phrase)
                .Include(t => t.SubmittedBy)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (translation == null)
            {
                return NotFound();
            }

            translation.Status = ContentStatus.Approved;
            translation.ReviewedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            var submitterId = translation.SubmittedById;
            if (!string.IsNullOrEmpty(submitterId))
            {
                var submitter = await _context.Users.FindAsync(submitterId);
                if (submitter != null)
                {
                    submitter.TotalApprovedTranslations += 1;
                    await _context.SaveChangesAsync();

                    // ✅ Check if they just became a Trusted Contributor
                    if (submitter.TotalApprovedTranslations == 10)
                    {
                        TempData["SuccessMessage"] = $"🎉 {submitter.FullName ?? submitter.Email} is now a Trusted Contributor!";
                    }
                    else
                    {
                        TempData["SuccessMessage"] = $"Translation '{translation.TranslatedText}' has been approved!";
                    }
                }
            }

            return RedirectToAction(nameof(Pending));
        }

        // GET: TranslationsAdmin/Reject/5
        public async Task<IActionResult> Reject(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole("Administrator");
            var isModerator = user?.IsModerator == true;

            if (!isAdmin && !isModerator)
            {
                return RedirectToAction("Index", "Home");
            }

            var translation = await _context.Translations
                .Include(t => t.Phrase)
                .Include(t => t.SubmittedBy)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (translation == null)
            {
                return NotFound();
            }

            return View(translation);
        }

        // POST: TranslationsAdmin/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string? reason)
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole("Administrator");
            var isModerator = user?.IsModerator == true;

            if (!isAdmin && !isModerator)
            {
                return RedirectToAction("Index", "Home");
            }

            var translation = await _context.Translations.FindAsync(id);
            if (translation == null)
            {
                return NotFound();
            }

            translation.Status = ContentStatus.Rejected;
            translation.AdminReviewComment = reason ?? "No reason provided";
            translation.ReviewedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Translation has been rejected.";
            return RedirectToAction(nameof(Pending));
        }

        // POST: TranslationsAdmin/BulkApprove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkApprove(int[] selectedIds)
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole("Administrator");
            var isModerator = user?.IsModerator == true;

            if (!isAdmin && !isModerator)
            {
                return RedirectToAction("Index", "Home");
            }

            if (selectedIds == null || !selectedIds.Any())
            {
                TempData["ErrorMessage"] = "No translations selected.";
                return RedirectToAction(nameof(Pending));
            }

            var translations = await _context.Translations
                .Include(t => t.SubmittedBy)
                .Where(t => selectedIds.Contains(t.Id) && t.Status == ContentStatus.Pending)
                .ToListAsync();

            foreach (var translation in translations)
            {
                translation.Status = ContentStatus.Approved;
                translation.ReviewedDate = DateTime.Now;

                if (translation.SubmittedBy != null)
                {
                    translation.SubmittedBy.TotalApprovedTranslations += 1;
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"{translations.Count} translations approved successfully!";
            return RedirectToAction(nameof(Pending));
        }

        // GET: TranslationsAdmin/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole("Administrator");
            var isModerator = user?.IsModerator == true;

            if (!isAdmin && !isModerator)
            {
                return RedirectToAction("Index", "Home");
            }

            var translation = await _context.Translations
                .Include(t => t.Phrase)
                .Include(t => t.SubmittedBy)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (translation == null)
            {
                return NotFound();
            }

            return View(translation);
        }
    }
}