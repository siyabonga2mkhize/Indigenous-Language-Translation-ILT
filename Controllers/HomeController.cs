using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhraseBookk.Data;
using PhraseBookk.Models;
using System.Diagnostics;

namespace PhraseBookk.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()

        {
           ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            var approvedPhrases = await _context.Phrases
                .Include(p => p.Category)
                .Include(p => p.Translations)
                .Where(p => p.IsActive && p.Translations.Any(t => t.Status == ContentStatus.Approved))
                .ToListAsync();

            if (approvedPhrases.Any())
            {
                var random = new Random();
                var randomIndex = random.Next(approvedPhrases.Count);
                ViewBag.PhraseOfTheDay = approvedPhrases[randomIndex];
            }

            if (User.Identity.IsAuthenticated && User.IsInRole("Administrator"))
            {
                ViewBag.PendingCount = await _context.Translations.CountAsync(t => t.Status == ContentStatus.Pending);
                ViewBag.TotalPhrases = await _context.Phrases.CountAsync();
                ViewBag.TotalTranslations = await _context.Translations.CountAsync();
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}