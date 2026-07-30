using InnoDevsITL.Data;
using InnoDevsITL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InnoDevsITL.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        private readonly InnoDbContext _context;
        private readonly UserManager<Users> _userManager;
        private readonly ILogger<TeacherController> _logger;

        public TeacherController(
            InnoDbContext context,
            UserManager<Users> userManager,
            ILogger<TeacherController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            _logger.LogInformation($"Teacher {currentUser?.Email} accessed dashboard");

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get teacher's teaching statistics
            var totalPhrases = await _context.Phrases
                .Where(p => p.IsActive)
                .CountAsync();

            var totalCategories = await _context.Categories.CountAsync();

            var totalStudents = await _userManager.GetUsersInRoleAsync("Student");

            var recentSubmissions = await _context.Submissions
                .Include(s => s.Phrase)
                .OrderByDescending(s => s.SubmittedAt)
                .Take(5)
                .ToListAsync();

            ViewBag.CurrentUser = currentUser;
            ViewBag.TotalPhrases = totalPhrases;
            ViewBag.TotalCategories = totalCategories;
            ViewBag.TotalStudents = totalStudents.Count;
            ViewBag.RecentSubmissions = recentSubmissions;

            return View();
        }

        public async Task<IActionResult> ManagePhrases()
        {
            var phrases = await _context.Phrases
                .Include(p => p.Category)
                .Include(p => p.Translations)
                .OrderBy(p => p.EnglishText)
                .ToListAsync();

            return View(phrases);
        }

        public async Task<IActionResult> ViewCategories()
        {
            var categories = await _context.Categories
                .Include(c => c.Phrases)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(categories);
        }

        public async Task<IActionResult> StudentProgress()
        {
            var students = await _userManager.GetUsersInRoleAsync("Student");
            var studentList = students.ToList();

            var progressData = new List<dynamic>();

            foreach (var student in studentList)
            {
                var submissions = await _context.Submissions
                    .Where(s => s.UserId == student.Id)
                    .CountAsync();

                var approved = await _context.Submissions
                    .Where(s => s.UserId == student.Id && s.IsApproved)
                    .CountAsync();

                progressData.Add(new
                {
                    Student = student,
                    TotalSubmissions = submissions,
                    ApprovedSubmissions = approved
                });
            }

            return View(progressData);
        }
    }
}
