using Microsoft.AspNetCore.Identity;

namespace PhraseBookk.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public bool IsModerator { get; set; } = false;
        public string? ModeratorLanguage { get; set; }


          public int TotalApprovedTranslations { get; set; } = 0;

        // Navigation properties
        public virtual ICollection<Favorite>? Favorites { get; set; }
        public virtual ICollection<Translation>? SubmittedTranslations { get; set; }
        public virtual ICollection<TranslationVote>? TranslationVotes { get; set; }
    }
}