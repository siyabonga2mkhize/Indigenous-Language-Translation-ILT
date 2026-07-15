using System.ComponentModel.DataAnnotations;

namespace InnoDevsITL.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Physical address is required")]
        public string PhysicalAddress { get; set; }

        [Required(ErrorMessage = "Please selct Faculty Id")]
        public int FacultyId { get; set; }

        [Required(ErrorMessage = "Please selct Campus Id")]
        public int CampusId { get; set; }


        [Required(ErrorMessage = "Password is required")]
        [StringLength(20, MinimumLength =8, ErrorMessage = "The {0} must be at {2} and at max {1} characterrs long. ")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = "Password does not match.")]
        public string Password { get; set; }


        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        public string ConfirmPassword { get; set; }
    }
}
