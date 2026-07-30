using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhraseBookk.Data;
using PhraseBookk.Models;

namespace PhraseBookk.Controllers
{
    [Authorize]
    public class VotesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public VotesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: Votes/Toggle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int translationId, bool isUpvote, string? returnUrl = null)
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            // Check if translation exists
            var translation = await _context.Translations
                .FirstOrDefaultAsync(t => t.Id == translationId);

            if (translation == null)
            {
                TempData["ErrorMessage"] = "Translation not found.";
                return Redirect(returnUrl ?? "/");
            }

            // Check if user already voted
            var existingVote = await _context.TranslationVotes
                .FirstOrDefaultAsync(v => v.TranslationId == translationId && v.UserId == user.Id);

            if (existingVote != null)
            {
                // If same vote type, remove vote (toggle off)
                if (existingVote.IsUpvote == isUpvote)
                {
                    _context.TranslationVotes.Remove(existingVote);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Vote removed.";
                    return Redirect(returnUrl ?? "/");
                }
                // If different vote type, update vote
                else
                {
                    existingVote.IsUpvote = isUpvote;
                    existingVote.CreatedDate = DateTime.Now;
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Vote updated!";
                    return Redirect(returnUrl ?? "/");
                }
            }

            // Create new vote
            var vote = new TranslationVote
            {
                TranslationId = translationId,
                UserId = user.Id,
                IsUpvote = isUpvote,
                CreatedDate = DateTime.Now
            };

            _context.TranslationVotes.Add(vote);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = isUpvote ? "👍 Upvoted!" : "👎 Downvoted!";
            return Redirect(returnUrl ?? "/");
        }

        // GET: Votes/GetVotes/5
        [HttpGet]
        public async Task<IActionResult> GetVotes(int translationId)
        {
            var votes = await _context.TranslationVotes
                .Where(v => v.TranslationId == translationId)
                .ToListAsync();

            var upvotes = votes.Count(v => v.IsUpvote);
            var downvotes = votes.Count(v => !v.IsUpvote);

            return Json(new { upvotes, downvotes, score = upvotes - downvotes });
        }
    }
}