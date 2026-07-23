using PhraseBookk.Models;

namespace PhraseBookk.Models
{
    public class Favorite
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public int PhraseId { get; set; }
        public DateTime SavedDate { get; set; } = DateTime.Now;

        public virtual ApplicationUser? User { get; set; }
        public virtual Phrase? Phrase { get; set; }
    }
}