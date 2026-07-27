using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhraseBookk.Data;
using PhraseBookk.Models;
using PhraseBookk.Services;
using System.Diagnostics;

namespace PhraseBookk.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IQRCodeService _qrCodeService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IQRCodeService qrCodeService)
        {
            _logger = logger;
            _context = context;
            _qrCodeService = qrCodeService;
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

        [HttpGet]
        public async Task<IActionResult> GenerateQRCode(int categoryId)
        {
            try
            {
                var category = await _context.Categories.FindAsync(categoryId);
                if (category == null)
                {
                    return NotFound();
                }

                var qrData = $"categoryId={categoryId}";
                var qrCodeDataUrl = _qrCodeService.GenerateQRCodeUrl(qrData);

                return Json(new { success = true, qrCode = qrCodeDataUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating QR code");
                return Json(new { success = false, message = "Error generating QR code" });
            }
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

        [HttpPost]
        public async Task<IActionResult> ProcessQRCode([FromBody] QRCodeData qrData)
        {
            if (string.IsNullOrEmpty(qrData?.Data))
            {
                return BadRequest("QR code data is required");
            }

            try
            {
                // Parse the QR code data (format: categoryId={id})
                if (qrData.Data.StartsWith("categoryId="))
                {
                    var categoryIdStr = qrData.Data.Replace("categoryId=", "");
                    if (int.TryParse(categoryIdStr, out int categoryId))
                    {
                        var category = await _context.Categories.FindAsync(categoryId);
                        if (category != null)
                        {
                            return Json(new { success = true, categoryId = categoryId, categoryName = category.Name });
                        }
                    }
                }

                return Json(new { success = false, message = "Invalid QR code format" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing QR code");
                return Json(new { success = false, message = "Error processing QR code" });
            }
        }
    }

    public class QRCodeData
    {
        public string? Data { get; set; }
    }
}