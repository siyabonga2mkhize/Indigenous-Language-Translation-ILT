using InnoDevsITL.Data;
using InnoDevsITL.Models;
using InnoDevsITL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InnoDevsITL.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly InnoDbContext _context;
        private readonly UserManager<Users> _userManager;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            InnoDbContext context, 
            UserManager<Users> userManager,
            ILogger<AdminController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            _logger.LogInformation($"Admin {currentUser?.Email} accessed dashboard");

            // Get dashboard statistics
            var totalUsers = await _userManager.Users.CountAsync();
            var pendingSubmissions = await _context.Submissions
                .Where(s => !s.IsApproved)
                .CountAsync();
            var totalPhrases = await _context.Phrases.CountAsync();
            var totalCategories = await _context.Categories.CountAsync();

            ViewBag.TotalUsers = totalUsers;
            ViewBag.PendingSubmissions = pendingSubmissions;
            ViewBag.TotalPhrases = totalPhrases;
            ViewBag.TotalCategories = totalCategories;
            ViewBag.CurrentUser = currentUser;

            return View();
        }

        // Review submissions
        public async Task<IActionResult> ReviewSubmissions()
        {
            var submissions = await _context.Submissions
                .Include(s => s.Phrase)
                .Include(s => s.Phrase.Category)
                .Where(s => !s.IsApproved)
                .OrderBy(s => s.SubmittedAt)
                .ToListAsync();

            return View(submissions);
        }

        public async Task<IActionResult> ReviewSubmission(int id)
        {
            var submission = await _context.Submissions
                .Include(s => s.Phrase)
                .Include(s => s.Phrase.Category)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (submission == null)
            {
                return NotFound();
            }

            var model = new ReviewViewModel
            {
                Submission = submission,
                Phrase = submission.Phrase
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveSubmission(int id, string reviewerNote)
        {
            var submission = await _context.Submissions
                .Include(s => s.Phrase)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (submission == null)
            {
                return NotFound();
            }

            var reviewer = await _userManager.GetUserAsync(User);

            // Approve the submission
            submission.IsApproved = true;
            submission.ReviewedBy = reviewer?.UserName ?? "Unknown";

            // Activate the phrase
            submission.Phrase.IsActive = true;

            // Add the translation to the phrase
            var translation = new Translation
            {
                Text = submission.SubmittedText,
                Language = submission.Phrase.Language,
                IsApproved = true,
                PhraseId = submission.Phrase.Id
            };

            _context.Translations.Add(translation);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Submission {id} approved by {reviewer?.Email}");
            TempData["Success"] = "Submission approved successfully!";
            return RedirectToAction(nameof(ReviewSubmissions));
        }

        [HttpPost]
        public async Task<IActionResult> RejectSubmission(int id, string reviewerNote)
        {
            var submission = await _context.Submissions
                .Include(s => s.Phrase)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (submission == null)
            {
                return NotFound();
            }

            var reviewer = await _userManager.GetUserAsync(User);

            // Remove the related phrase
            if (submission.Phrase != null)
            {
                _context.Phrases.Remove(submission.Phrase);
            }

            // Remove the submission
            _context.Submissions.Remove(submission);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Submission {id} rejected by {reviewer?.Email}");
            TempData["Info"] = "Submission rejected and removed.";
            return RedirectToAction(nameof(ReviewSubmissions));
        }

        // Manage Users/Teachers
        public async Task<IActionResult> ManageUsers()
        {
            var users = await _userManager.Users
                .Include(u => u.Faculty)
                .Include(u => u.Campus)
                .ToListAsync();

            var userRoles = new List<dynamic>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles.Add(new { User = user, Roles = roles });
            }

            ViewBag.UserRoles = userRoles;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyTeacher(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Add to Teacher role
            await _userManager.AddToRoleAsync(user, "Teacher");
            _logger.LogInformation($"User {user.Email} promoted to Teacher by admin");
            TempData["Success"] = "Teacher verified successfully!";
            return RedirectToAction(nameof(ManageUsers));
        }
    }
}
