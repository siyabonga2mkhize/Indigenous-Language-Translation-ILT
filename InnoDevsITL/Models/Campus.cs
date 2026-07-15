using System.ComponentModel.DataAnnotations;

namespace InnoDevsITL.Models
{
    public class Campus
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
