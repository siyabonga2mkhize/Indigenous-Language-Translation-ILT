
using PhraseBookk.Models;
using System.ComponentModel.DataAnnotations;

namespace PhraseBookk.Models
{
    public class UsageStat
    {
        public int Id { get; set; }

        [Required]
        public LanguageCode LanguageSelected { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public string? SearchKeyword { get; set; }
        public string? UserId { get; set; }
        public DateTime ViewedAt { get; set; } = DateTime.Now;

        public virtual Category? Category { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}