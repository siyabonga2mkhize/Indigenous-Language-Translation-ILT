using System.ComponentModel.DataAnnotations;
using PhraseBookk.Models;

namespace PhraseBookk.ViewModels
{
    public class TranslationSubmissionViewModel
    {
        public int PhraseId { get; set; }

        [Required(ErrorMessage = "Please select a language")]
        public LanguageCode Language { get; set; }

        [Required(ErrorMessage = "Please enter the translation")]
        [Display(Name = "Translated Text")]
        public string TranslatedText { get; set; } = string.Empty;

        public string? PhraseEnglishText { get; set; }
        public string? PhraseCategoryName { get; set; }
    }
}