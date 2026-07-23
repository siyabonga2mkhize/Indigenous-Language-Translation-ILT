using PhraseBookk.Models;
using System.ComponentModel.DataAnnotations;

namespace PhraseBookk.Models
{
    public class Phrase
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "English Text")]
        public string EnglishText { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int CategoryId { get; set; }
        public string? CreatedById { get; set; }

        public virtual Category? Category { get; set; }
        public virtual ApplicationUser? CreatedBy { get; set; }
        public virtual ICollection<Translation>? Translations { get; set; }
        public virtual ICollection<Favorite>? FavoritedByUsers { get; set; }
    }
}