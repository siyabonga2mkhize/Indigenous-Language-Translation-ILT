using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhraseBookk.Data;
using PhraseBookk.Models;

namespace PhraseBookk.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class PhrasesAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PhrasesAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PhrasesAdmin
        public async Task<IActionResult> Index()
        {
            var phrases = await _context.Phrases
                .Include(p => p.Category)
                .Include(p => p.Translations)
                .ToListAsync();

            ViewBag.TotalCategories = await _context.Categories.CountAsync();
            ViewBag.PendingCount = await _context.Translations.CountAsync(t => t.Status == ContentStatus.Pending);
            ViewBag.TotalTranslations = await _context.Translations.CountAsync();
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.TotalPhrases = phrases.Count;

            var recentActivity = await _context.Translations
                .Include(t => t.SubmittedBy)
                .Include(t => t.Phrase)
                .OrderByDescending(t => t.CreatedDate)
                .Take(5)
                .ToListAsync();

            ViewBag.RecentActivity = recentActivity.Select(t =>
                $"{(t.SubmittedBy?.FullName ?? "User")} submitted translation for \"{t.Phrase?.EnglishText}\""
            ).ToList();

            return View(phrases);
        }

        // GET: PhrasesAdmin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phrase = await _context.Phrases
                .Include(p => p.Category)
                .Include(p => p.Translations)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (phrase == null)
            {
                return NotFound();
            }

            return View(phrase);
        }

        // GET: PhrasesAdmin/Create
        public IActionResult Create()
        {
            // ✅ FIXED: Use SelectList instead of raw list
            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
            return View();
        }

        // POST: PhrasesAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EnglishText,CategoryId,IsActive")] Phrase phrase)
        {
            if (ModelState.IsValid)
            {
                phrase.CreatedDate = DateTime.Now;
                _context.Add(phrase);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Phrase created successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name", phrase.CategoryId);
            return View(phrase);
        }

        // GET: PhrasesAdmin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phrase = await _context.Phrases.FindAsync(id);
            if (phrase == null)
            {
                return NotFound();
            }
            // ✅ FIXED: Use SelectList with the current selected value
            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name", phrase.CategoryId);
            return View(phrase);
        }

        // POST: PhrasesAdmin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EnglishText,CategoryId,IsActive,CreatedDate")] Phrase phrase)
        {
            if (id != phrase.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phrase);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Phrase updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhraseExists(phrase.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name", phrase.CategoryId);
            return View(phrase);
        }

        // GET: PhrasesAdmin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phrase = await _context.Phrases
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (phrase == null)
            {
                return NotFound();
            }

            return View(phrase);
        }

        // POST: PhrasesAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var phrase = await _context.Phrases.FindAsync(id);
            if (phrase != null)
            {
                _context.Phrases.Remove(phrase);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Phrase deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PhraseExists(int id)
        {
            return _context.Phrases.Any(e => e.Id == id);
        }
    }
}