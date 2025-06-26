using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryProject.Models
{
    public enum MemberStatus
    {
        Active,
        Suspended,
        Expired
    }

    public class Members
    {
        public int Id { get; set; } // Auto-generated

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        public string Address { get; set; }

        [Display(Name = "Registration Date")]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [Display(Name = "Status")]
        public MemberStatus Status { get; set; } = MemberStatus.Active;
    }
}
