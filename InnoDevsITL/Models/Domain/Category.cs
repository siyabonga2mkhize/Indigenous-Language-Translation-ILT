using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InnoDevsITL.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Phrase> Phrases { get; set; } = new List<Phrase>();
    }
}
