using System.ComponentModel.DataAnnotations;

namespace InnoDevsITL.Models
{
    public class Favourite
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public int PhraseId { get; set; }
        public Phrase Phrase { get; set; }
    }
}
