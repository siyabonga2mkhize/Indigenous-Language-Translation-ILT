using PhraseBookk.Models;
using System.ComponentModel.DataAnnotations;

namespace PhraseBookk.Models
{
    public class Translation
    {
        public int Id { get; set; }

        [Required]
        public LanguageCode Language { get; set; }

        [Required]
        [Display(Name = "Translated Text")]
        public string TranslatedText { get; set; } = string.Empty;

        public ContentStatus Status { get; set; } = ContentStatus.Pending;

        [Display(Name = "Admin Feedback")]
        public string? AdminReviewComment { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ReviewedDate { get; set; }

        public int PhraseId { get; set; }
        public string? SubmittedById { get; set; }

        public virtual Phrase? Phrase { get; set; }
        public virtual ApplicationUser? SubmittedBy { get; set; }
    }
}
