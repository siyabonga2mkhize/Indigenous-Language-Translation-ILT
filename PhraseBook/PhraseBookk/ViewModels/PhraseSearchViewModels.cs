using PhraseBookk.Models;

namespace PhraseBookk.ViewModels
{
    public class PhraseSearchViewModel
    {
        public string? SearchKeyword { get; set; }
        public int? CategoryId { get; set; }
        public List<Category> Categories { get; set; } = new();
        public List<PhraseResultViewModel> Results { get; set; } = new();
    }

    public class PhraseResultViewModel
    {
        public int PhraseId { get; set; }
        public string EnglishText { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public List<MatchedTranslation> MatchedTranslations { get; set; } = new();
        public bool IsFavorited { get; set; }
    }

    public class MatchedTranslation
    {
        public string LanguageName { get; set; } = string.Empty;
        public string TranslatedText { get; set; } = string.Empty;
        public string HighlightedText { get; set; } = string.Empty;
    }
}