using InnoDevsITL.Data;
using InnoDevsITL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InnoDevsITL.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly InnoDbContext _context;
        private readonly UserManager<Users> _userManager;
        private readonly ILogger<StudentController> _logger;

        public StudentController(
            InnoDbContext context,
            UserManager<Users> userManager,
            ILogger<StudentController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            _logger.LogInformation($"Student {currentUser?.Email} accessed dashboard");

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get student's learning statistics
            var userSubmissions = await _context.Submissions
                .Where(s => s.UserId == currentUser.Id)
                .CountAsync();

            var approvedSubmissions = await _context.Submissions
                .Where(s => s.UserId == currentUser.Id && s.IsApproved)
                .CountAsync();

            var userFavourites = await _context.Favourites
                .Where(f => f.UserId == currentUser.Id)
                .CountAsync();

            var recentPhrases = await _context.Phrases
                .Where(p => p.IsActive)
                .Include(p => p.Category)
                .OrderByDescending(p => p.Id)
                .Take(5)
                .ToListAsync();

            ViewBag.CurrentUser = currentUser;
            ViewBag.TotalSubmissions = userSubmissions;
            ViewBag.ApprovedSubmissions = approvedSubmissions;
            ViewBag.TotalFavourites = userFavourites;
            ViewBag.RecentPhrases = recentPhrases;

            return View();
        }

        public async Task<IActionResult> MySubmissions()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var submissions = await _context.Submissions
                .Where(s => s.UserId == currentUser.Id)
                .Include(s => s.Phrase)
                .Include(s => s.Phrase.Category)
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();

            return View(submissions);
        }

        public async Task<IActionResult> MyFavourites()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var favourites = await _context.Favourites
                .Where(f => f.UserId == currentUser.Id)
                .Include(f => f.Phrase)
                .Include(f => f.Phrase.Category)
                .Include(f => f.Phrase.Translations)
                .ToListAsync();

            return View(favourites);
        }
    }
}
