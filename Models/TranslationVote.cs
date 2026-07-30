using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhraseBookk.Models
{
    public class TranslationVote
    {
        public int Id { get; set; }

        [Required]
        public int TranslationId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public bool IsUpvote { get; set; } // true = 👍, false = 👎

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("TranslationId")]
        public virtual Translation? Translation { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
    }
}