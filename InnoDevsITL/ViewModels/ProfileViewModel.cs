using System.ComponentModel.DataAnnotations;

namespace InnoDevsITL.ViewModels
{
    public class ProfileViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string PhysicalAddress { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public int FacultyId { get; set; }
        public int CampusId { get; set; }

        [Display(Name = "Profile Picture")]
        public IFormFile? ProfilePicture { get; set; }

        public string? ProfilePictureUrl { get; set; }
    }
}