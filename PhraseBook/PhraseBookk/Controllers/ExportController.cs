using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhraseBookk.Data;
using PhraseBookk.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace PhraseBookk.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ExportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ExportController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Export/Index
        public async Task<IActionResult> Index()
        {
            ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            return View();
        }

        // POST: Export/Phrasebook
        [HttpPost]
        public async Task<IActionResult> Phrasebook(int? categoryId)
        {
            // Get phrases based on category filter
            var query = _context.Phrases
                .Include(p => p.Category)
                .Include(p => p.Translations)
                .Where(p => p.IsActive);

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            var phrases = await query.OrderBy(p => p.Category.Name).ThenBy(p => p.EnglishText).ToListAsync();

            using (var stream = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 50, 50, 50, 50);
                var writer = PdfWriter.GetInstance(document, stream);

                // Add page number footer
                writer.PageEvent = new CustomPdfPageEvent
                {
                    FooterText = "PhraseBook - Durban University of Technology"
                };

                document.Open();

                // ---------- COVER PAGE ----------
                // Try to load logo (PNG first, then JPEG)
                Image logo = null;
                var pngPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "dut-logo.png");
                var jpgPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "dut-logo.jpeg");

                if (System.IO.File.Exists(pngPath))
                {
                    try
                    {
                        logo = Image.GetInstance(pngPath);
                    }
                    catch { /* ignore */ }
                }

                if (logo == null && System.IO.File.Exists(jpgPath))
                {
                    try
                    {
                        logo = Image.GetInstance(jpgPath);
                    }
                    catch { /* ignore */ }
                }

                if (logo != null)
                {
                    logo.ScaleToFit(150f, 80f);
                    logo.Alignment = Element.ALIGN_CENTER;
                    logo.SpacingAfter = 20;
                    document.Add(logo);
                }
                else
                {
                    // Fallback: Show university name as text
                    var fallbackFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20, new BaseColor(0, 51, 102));
                    var fallback = new Paragraph("DURBAN UNIVERSITY OF TECHNOLOGY", fallbackFont);
                    fallback.Alignment = Element.ALIGN_CENTER;
                    fallback.SpacingAfter = 15;
                    document.Add(fallback);
                }

                // Title
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 28, new BaseColor(0, 51, 102));
                var title = new Paragraph("📚 CAMPUS PHRASEBOOK", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 10;
                document.Add(title);

                // Subtitle
                var subtitleFont = FontFactory.GetFont(FontFactory.HELVETICA, 16, BaseColor.GRAY);
                var subtitle = new Paragraph("Durban University of Technology", subtitleFont);
                subtitle.Alignment = Element.ALIGN_CENTER;
                subtitle.SpacingAfter = 20;
                document.Add(subtitle);

                // Description
                var descFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.DARK_GRAY);
                var desc = new Paragraph(
                    "A comprehensive collection of campus phrases in South Africa's 11 official languages.\n" +
                    "This handbook helps students navigate registration, accommodation, health services,\n" +
                    "library, and academic support with ease.",
                    descFont);
                desc.Alignment = Element.ALIGN_CENTER;
                desc.SpacingAfter = 30;
                document.Add(desc);

                // Metadata
                var metaFont = FontFactory.GetFont(FontFactory.HELVETICA, 11, BaseColor.GRAY);
                var meta = new Paragraph(
                    $"Generated: {DateTime.Now:dd MMMM yyyy HH:mm}\n" +
                    $"Total Phrases: {phrases.Count}\n" +
                    $"Categories: {phrases.Select(p => p.Category?.Name).Distinct().Count()}\n" +
                    $"Languages: 11",
                    metaFont);
                meta.Alignment = Element.ALIGN_CENTER;
                meta.SpacingAfter = 40;
                document.Add(meta);

                // Decorative line
                var line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.5f, 100f, BaseColor.LIGHT_GRAY, Element.ALIGN_CENTER, -1)));
                document.Add(line);

                document.NewPage();

                // ---------- CONTENT PAGE ----------
                var headingFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, new BaseColor(0, 51, 102));
                var categoryFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, new BaseColor(0, 51, 102));
                var phraseFont = FontFactory.GetFont(FontFactory.HELVETICA, 11, BaseColor.BLACK);
                var transFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.DARK_GRAY);

                var sectionTitle = new Paragraph("PHRASE DIRECTORY", headingFont);
                sectionTitle.Alignment = Element.ALIGN_CENTER;
                sectionTitle.SpacingAfter = 20;
                document.Add(sectionTitle);

                // Group by category
                var grouped = phrases.GroupBy(p => p.Category?.Name ?? "Uncategorized");
                foreach (var group in grouped.OrderBy(g => g.Key))
                {
                    // Category header
                    var catHeader = new Paragraph($"📂 {group.Key}", categoryFont);
                    catHeader.SpacingBefore = 15;
                    catHeader.SpacingAfter = 5;
                    document.Add(catHeader);

                    // Underline
                    var underline = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.5f, 80f, BaseColor.LIGHT_GRAY, Element.ALIGN_LEFT, -1)));
                    document.Add(underline);

                    foreach (var phrase in group.OrderBy(p => p.EnglishText))
                    {
                        // Phrase
                        var phraseText = new Paragraph($"▶ {phrase.EnglishText}", phraseFont);
                        phraseText.IndentationLeft = 20;
                        phraseText.SpacingBefore = 8;
                        document.Add(phraseText);

                        // Translations
                        var approvedTranslations = phrase.Translations?.Where(t => t.Status == ContentStatus.Approved).ToList();
                        if (approvedTranslations != null && approvedTranslations.Any())
                        {
                            foreach (var trans in approvedTranslations)
                            {
                                var transLine = new Paragraph($"   • {trans.Language}: {trans.TranslatedText}", transFont);
                                transLine.IndentationLeft = 40;
                                transLine.SpacingBefore = 2;
                                document.Add(transLine);
                            }
                        }
                    }
                }

                document.Close();
                return File(stream.ToArray(), "application/pdf", $"DUT_PhraseBook_{DateTime.Now:yyyyMMdd}.pdf");
            }
        }
    }

    // Custom PDF event helper
    public class CustomPdfPageEvent : iTextSharp.text.pdf.PdfPageEventHelper
    {
        public string FooterText { get; set; } = "PhraseBook - DUT";

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);
            var cb = writer.DirectContent;
            var ph = new Paragraph(FooterText + " | Page " + writer.PageNumber,
                FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.GRAY));
            ph.Alignment = Element.ALIGN_CENTER;
            ph.SpacingBefore = 10;
            var table = new PdfPTable(1);
            table.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            table.DefaultCell.Border = 0;
            table.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(ph);
            table.WriteSelectedRows(0, -1, document.LeftMargin, document.BottomMargin - 10, cb);
        }
    }
}