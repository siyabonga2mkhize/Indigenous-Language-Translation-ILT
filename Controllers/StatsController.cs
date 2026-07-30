using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhraseBookk.Data;
using PhraseBookk.Models;

namespace PhraseBookk.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class StatsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Get usage stats grouped by language
            var languageStats = await _context.UsageStats
                .Where(u => u.LanguageSelected != null)
                .GroupBy(u => u.LanguageSelected)
                .Select(g => new { Language = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .ToListAsync();

            // Get usage stats grouped by category
            var categoryStats = await _context.UsageStats
                .Where(u => u.Category != null)
                .GroupBy(u => u.Category)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .ToListAsync();

            // Get total counts
            var totalSearches = await _context.UsageStats.CountAsync();
            var totalPhrases = await _context.Phrases.CountAsync();
            var totalTranslations = await _context.Translations.CountAsync();
            var pendingTranslations = await _context.Translations.CountAsync(t => t.Status == ContentStatus.Pending);
            var totalUsers = await _context.Users.CountAsync();

            ViewBag.LanguageStats = languageStats;
            ViewBag.CategoryStats = categoryStats;
            ViewBag.TotalSearches = totalSearches;
            ViewBag.TotalPhrases = totalPhrases;
            ViewBag.TotalTranslations = totalTranslations;
            ViewBag.PendingTranslations = pendingTranslations;
            ViewBag.TotalUsers = totalUsers;

            return View();
        }
    }
}