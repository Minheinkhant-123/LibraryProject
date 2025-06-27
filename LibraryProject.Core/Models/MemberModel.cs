using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryProject.Models
{
    public enum MemberStatus
    {
        Active = 1,
        Suspended = 2,
        Expired = 3,
        Deleted = 0
    }

    public class Members
    {
        public int Id { get; set; }

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
