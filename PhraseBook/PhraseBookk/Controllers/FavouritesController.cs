using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhraseBookk.Data;
using PhraseBookk.Models;

namespace PhraseBookk.Controllers
{
    [Authorize]  // Requires user to be logged in
    public class FavouritesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FavouritesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Favourites/Index
        public async Task<IActionResult> Index()
        {
            var userId = _context.Users.FirstOrDefault(u => u.UserName == User.Identity!.Name)?.Id;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var favourites = await _context.Favorites
                .Include(f => f.Phrase)
                .ThenInclude(p => p.Category)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.SavedDate)
                .ToListAsync();

            return View(favourites);
        }

        // POST: Favourites/Toggle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int phraseId)
        {
            var userId = _context.Users.FirstOrDefault(u => u.UserName == User.Identity!.Name)?.Id;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Please log in first." });
            }

            // Check if phrase exists and is active
            var phrase = await _context.Phrases.FirstOrDefaultAsync(p => p.Id == phraseId && p.IsActive);
            if (phrase == null)
            {
                return Json(new { success = false, message = "Phrase not found." });
            }

            // Check if already favourited
            var existing = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.PhraseId == phraseId);

            if (existing != null)
            {
                // Remove from favourites
                _context.Favorites.Remove(existing);
                await _context.SaveChangesAsync();
                return Json(new { success = true, action = "removed", message = "Removed from favourites." });
            }
            else
            {
                // Add to favourites
                var favorite = new Favorite
                {
                    UserId = userId,
                    PhraseId = phraseId,
                    SavedDate = DateTime.Now
                };
                _context.Favorites.Add(favorite);
                await _context.SaveChangesAsync();
                return Json(new { success = true, action = "added", message = "Added to favourites!" });
            }
        }

        // POST: Favourites/Remove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var userId = _context.Users.FirstOrDefault(u => u.UserName == User.Identity!.Name)?.Id;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);

            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Removed from favourites.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Favourites/Count
        public async Task<IActionResult> Count()
        {
            var userId = _context.Users.FirstOrDefault(u => u.UserName == User.Identity!.Name)?.Id;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { count = 0 });
            }

            var count = await _context.Favorites.CountAsync(f => f.UserId == userId);
            return Json(new { count = count });
        }
    }
}
