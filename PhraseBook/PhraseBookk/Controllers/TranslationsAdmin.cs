using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhraseBookk.Data;
using PhraseBookk.Models;

namespace PhraseBookk.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class TranslationsAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TranslationsAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TranslationsAdmin/Pending
        public async Task<IActionResult> Pending()
        {
            var pendingTranslations = await _context.Translations
                .Include(t => t.Phrase)
                .Include(t => t.SubmittedBy)
                .Where(t => t.Status == ContentStatus.Pending)
                .OrderBy(t => t.CreatedDate)
                .ToListAsync();

            return View(pendingTranslations);
        }

        // GET: TranslationsAdmin/Approve/5
        public async Task<IActionResult> Approve(int id)
        {
            var translation = await _context.Translations
                .Include(t => t.Phrase)
                .Include(t => t.SubmittedBy)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (translation == null)
            {
                return NotFound();
            }

            // Update status to Approved
            translation.Status = ContentStatus.Approved;
            translation.ReviewedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Translation '{translation.TranslatedText}' has been approved!";
            return RedirectToAction(nameof(Pending));
        }

        // GET: TranslationsAdmin/Reject/5
        public async Task<IActionResult> Reject(int id)
        {
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
            if (selectedIds == null || !selectedIds.Any())
            {
                TempData["ErrorMessage"] = "No translations selected.";
                return RedirectToAction(nameof(Pending));
            }

            var translations = await _context.Translations
                .Where(t => selectedIds.Contains(t.Id) && t.Status == ContentStatus.Pending)
                .ToListAsync();

            foreach (var translation in translations)
            {
                translation.Status = ContentStatus.Approved;
                translation.ReviewedDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"{translations.Count} translations approved successfully!";
            return RedirectToAction(nameof(Pending));
        }

        // GET: TranslationsAdmin/Details/5
        public async Task<IActionResult> Details(int id)
        {
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

