using System.ComponentModel.DataAnnotations;

namespace PhraseBookk.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        // ✅ NEW: Location fields for campus map
        public string? Latitude { get; set; }      // e.g., "-29.8587"
        public string? Longitude { get; set; }     // e.g., "31.0218"
        public string? MapLocationName { get; set; } // e.g., "Registration Building"

        public virtual ICollection<Phrase>? Phrases { get; set; }
    }
}