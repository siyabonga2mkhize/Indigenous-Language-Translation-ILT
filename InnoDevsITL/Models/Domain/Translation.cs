using System.ComponentModel.DataAnnotations;

namespace InnoDevsITL.Models
{
    public class Translation
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public string Language { get; set; }

        public bool IsApproved { get; set; }

        public int PhraseId { get; set; }
        public Phrase Phrase { get; set; }
    }
}
