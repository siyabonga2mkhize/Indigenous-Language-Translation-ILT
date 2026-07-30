using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        // Audio URL property
        [Display(Name = "Audio URL")]
        public string? AudioUrl { get; set; }

        // ✅ Navigation property for votes
        public virtual ICollection<TranslationVote>? Votes { get; set; }

        // ✅ Computed properties (not stored in DB)
        [NotMapped]
        public int UpvoteCount => Votes?.Count(v => v.IsUpvote) ?? 0;

        [NotMapped]
        public int DownvoteCount => Votes?.Count(v => !v.IsUpvote) ?? 0;

        [NotMapped]
        public int Score => UpvoteCount - DownvoteCount;

        // Navigation properties
        public virtual Phrase? Phrase { get; set; }
        public virtual ApplicationUser? SubmittedBy { get; set; }
    }
}
