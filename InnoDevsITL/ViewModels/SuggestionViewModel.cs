using System.ComponentModel.DataAnnotations;

namespace InnoDevsITL.ViewModels
{
    public class SuggestionViewModel
    {
        [Required]
        public string EnglishText { get; set; } = string.Empty;

        [Required]
        public string TranslationText { get; set; } = string.Empty;

        [Required]
        public string TargetLanguage { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }
    }
}