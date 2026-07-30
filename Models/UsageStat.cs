using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhraseBookk.Models
{
    public class UsageStat
    {
        public int Id { get; set; }

        public string? UserId { get; set; }
        public int? PhraseId { get; set; }
        public string? SearchKeyword { get; set; }
        public string? LanguageCode { get; set; }

        // ✅ Properties for statistics
        public string? LanguageSelected { get; set; }
        public int? CategoryId { get; set; }
        public string? Category { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string? Action { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        [ForeignKey("PhraseId")]
        public virtual Phrase? Phrase { get; set; }
    }
}