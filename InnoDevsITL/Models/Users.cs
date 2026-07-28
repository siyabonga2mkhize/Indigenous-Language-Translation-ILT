using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace InnoDevsITL.Models
{
    public class Users : IdentityUser
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [DataType(DataType.DateTime)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string PhysicalAddress { get; set; } = string.Empty;

        public string? ProfilePictureUrl { get; set; } // Nullable

        //Foreign Key for Faculty
        public virtual Faculty? Faculty { get; set; } // Nullable
        public int FacultyId { get; set; }

        //Foreign Key for Campus
        public virtual Campus? Campus { get; set; } // Nullable
        public int CampusId { get; set; }
    }
}