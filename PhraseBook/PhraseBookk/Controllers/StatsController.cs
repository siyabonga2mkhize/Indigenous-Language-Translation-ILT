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
            // Total searches
            var totalSearches = await _context.UsageStats.CountAsync();
            var totalViews = await _context.UsageStats.CountAsync(s => s.SearchKeyword == null); // Views are logged with null keyword

            // Languages
            var languageStats = await _context.UsageStats
                .GroupBy(s => s.LanguageSelected)
                .Select(g => new { Language = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .ToListAsync();

            // Categories
            var categoryStats = await _context.UsageStats
                .Include(s => s.Category)
                .GroupBy(s => s.CategoryId)
                .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .ToListAsync();

            // Get category names for display
            var categoryDict = await _context.Categories.ToDictionaryAsync(c => c.Id, c => c.Name);

            // Top keywords
            var keywordStats = await _context.UsageStats
                .Where(s => s.SearchKeyword != null && s.SearchKeyword != "")
                .GroupBy(s => s.SearchKeyword)
                .Select(g => new { Keyword = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .Take(10)
                .ToListAsync();

            // Count distinct languages and categories
            var distinctLanguages = languageStats.Count;
            var distinctCategories = categoryStats.Count;

            ViewBag.TotalSearches = totalSearches;
            ViewBag.TotalViews = totalViews;
            ViewBag.DistinctLanguages = distinctLanguages;
            ViewBag.DistinctCategories = distinctCategories;
            ViewBag.LanguageStats = languageStats;
            ViewBag.CategoryStats = categoryStats;
            ViewBag.CategoryDict = categoryDict;
            ViewBag.KeywordStats = keywordStats;

            return View();
        }
    }
}