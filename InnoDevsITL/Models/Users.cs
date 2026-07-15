using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace InnoDevsITL.Models
{
    public class Users : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string PhysicalAddress { get; set; }


        //Foreign Key for Faculty
        public virtual Faculty Faculty { get; set; }
        public int FacultyId { get; set; }


        //Foreign Key for Campus
        public virtual Campus Campus { get; set; }
        public int CampusId { get; set; }
        

    }


}


