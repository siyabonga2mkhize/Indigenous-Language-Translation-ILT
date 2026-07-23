using System;
using System.ComponentModel.DataAnnotations;

namespace InnoDevsITL.Models
{
    public class Submission
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string SubmittedText { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public bool IsApproved { get; set; }

        public string ReviewedBy { get; set; }

        public int PhraseId { get; set; }
        public Phrase Phrase { get; set; }
    }
