using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InnoDevsITL.Models
{
    public class Phrase
    {
        public int Id { get; set; }

        [Required]
        public string EnglishText { get; set; }

        [Required]
        public string Language { get; set; }

        public string Transcription { get; set; }

        public bool IsActive { get; set; } = true;

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<Translation> Translations { get; set; } = new List<Translation>();
    }
}
