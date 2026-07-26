using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace PhraseBookk.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public virtual ICollection<Favorite>? Favorites { get; set; }
        public virtual ICollection<Translation>? SubmittedTranslations { get; set; }
    }
}
