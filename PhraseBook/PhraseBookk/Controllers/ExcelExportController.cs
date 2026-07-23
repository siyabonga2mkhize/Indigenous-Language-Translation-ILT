using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PhraseBookk.Data;
using PhraseBookk.Models;
using System.Drawing;

namespace PhraseBookk.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ExcelExportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExcelExportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ExcelExport/Index
        public IActionResult Index()
        {
            return View();
        }

        // POST: ExcelExport/ExportPhrases
        [HttpPost]
        public async Task<IActionResult> ExportPhrases()
        {
            var phrases = await _context.Phrases
                .Include(p => p.Category)
                .Include(p => p.Translations)
                .Where(p => p.IsActive)
                .OrderBy(p => p.Category.Name)
                .ToListAsync();

            using (var package = new ExcelPackage())
            {
                // Sheet 1: Phrases
                var worksheet = package.Workbook.Worksheets.Add("Phrases");

                // Headers
                worksheet.Cells[1, 1].Value = "English Text";
                worksheet.Cells[1, 2].Value = "Category";
                worksheet.Cells[1, 3].Value = "Status";
                worksheet.Cells[1, 4].Value = "Translations";
                worksheet.Cells[1, 5].Value = "Created Date";

                using (var range = worksheet.Cells[1, 1, 1, 5])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                }

                // Data
                int row = 2;
                foreach (var phrase in phrases)
                {
                    worksheet.Cells[row, 1].Value = phrase.EnglishText;
                    worksheet.Cells[row, 2].Value = phrase.Category?.Name;
                    worksheet.Cells[row, 3].Value = phrase.IsActive ? "Active" : "Inactive";
                    worksheet.Cells[row, 4].Value = phrase.Translations?.Count ?? 0;
                    worksheet.Cells[row, 5].Value = phrase.CreatedDate.ToString("dd MMM yyyy");
                    row++;
                }

                worksheet.Cells.AutoFitColumns();

                // Sheet 2: Translations
                var translations = await _context.Translations
                    .Include(t => t.Phrase)
                    .Where(t => t.Status == ContentStatus.Approved)
                    .ToListAsync();

                var sheet2 = package.Workbook.Worksheets.Add("Translations");

                sheet2.Cells[1, 1].Value = "Phrase";
                sheet2.Cells[1, 2].Value = "Language";
                sheet2.Cells[1, 3].Value = "Translation";
                sheet2.Cells[1, 4].Value = "Status";

                using (var range = sheet2.Cells[1, 1, 1, 4])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                }

                row = 2;
                foreach (var trans in translations)
                {
                    sheet2.Cells[row, 1].Value = trans.Phrase?.EnglishText;
                    sheet2.Cells[row, 2].Value = trans.Language.ToString();
                    sheet2.Cells[row, 3].Value = trans.TranslatedText;
                    sheet2.Cells[row, 4].Value = trans.Status.ToString();
                    row++;
                }

                sheet2.Cells.AutoFitColumns();

                // Sheet 3: Statistics Summary
                var totalPhrases = await _context.Phrases.CountAsync();
                var totalTranslations = await _context.Translations.CountAsync();
                var totalCategories = await _context.Categories.CountAsync();
                var pendingCount = await _context.Translations.CountAsync(t => t.Status == ContentStatus.Pending);

                var sheet3 = package.Workbook.Worksheets.Add("Statistics");

                sheet3.Cells[1, 1].Value = "Statistic";
                sheet3.Cells[1, 2].Value = "Value";

                using (var range = sheet3.Cells[1, 1, 1, 2])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightYellow);
                }

                sheet3.Cells[2, 1].Value = "Total Phrases";
                sheet3.Cells[2, 2].Value = totalPhrases;
                sheet3.Cells[3, 1].Value = "Total Translations";
                sheet3.Cells[3, 2].Value = totalTranslations;
                sheet3.Cells[4, 1].Value = "Total Categories";
                sheet3.Cells[4, 2].Value = totalCategories;
                sheet3.Cells[5, 1].Value = "Pending Translations";
                sheet3.Cells[5, 2].Value = pendingCount;

                sheet3.Cells.AutoFitColumns();

                // Generate file
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"PhraseBook_Export_{DateTime.Now:yyyyMMdd}.xlsx");
            }
        }

        // POST: ExcelExport/ExportStats
        [HttpPost]
        public async Task<IActionResult> ExportStats()
        {
            var languageStats = await _context.UsageStats
                .GroupBy(s => s.LanguageSelected)
                .Select(g => new { Language = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .ToListAsync();

            var categoryStats = await _context.UsageStats
                .Include(s => s.Category)
                .GroupBy(s => s.CategoryId)
                .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .ToListAsync();

            var categoryDict = await _context.Categories.ToDictionaryAsync(c => c.Id, c => c.Name);

            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("Usage Statistics");

                // Headers
                sheet.Cells[1, 1].Value = "Language";
                sheet.Cells[1, 2].Value = "Search Count";

                using (var range = sheet.Cells[1, 1, 1, 2])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                }

                int row = 2;
                foreach (var item in languageStats)
                {
                    sheet.Cells[row, 1].Value = item.Language.ToString();
                    sheet.Cells[row, 2].Value = item.Count;
                    row++;
                }

                row++;
                sheet.Cells[row, 1].Value = "Category";
                sheet.Cells[row, 2].Value = "Search Count";

                using (var range = sheet.Cells[row, 1, row, 2])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                }

                row++;
                foreach (var item in categoryStats)
                {
                    var catName = categoryDict.ContainsKey(item.CategoryId) ? categoryDict[item.CategoryId] : "Unknown";
                    sheet.Cells[row, 1].Value = catName;
                    sheet.Cells[row, 2].Value = item.Count;
                    row++;
                }

                sheet.Cells.AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"PhraseBook_Stats_{DateTime.Now:yyyyMMdd}.xlsx");
            }
        }
    }
}

