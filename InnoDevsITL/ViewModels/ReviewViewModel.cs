using InnoDevsITL.Models;

namespace InnoDevsITL.ViewModels
{
    public class ReviewViewModel
    {
        public Submission Submission { get; set; } = new Submission();
        public Phrase Phrase { get; set; } = new Phrase();
        public string ReviewerNote { get; set; } = string.Empty;
    }
}