using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhraseBookk.Models
{
    public class Favorite
    {
        public int Id { get; set; }

        [Required]
        public int PhraseId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        // ✅ CHANGE: Use CreatedDate to match the database
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [ForeignKey("PhraseId")]
        public virtual Phrase? Phrase { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
    }
}