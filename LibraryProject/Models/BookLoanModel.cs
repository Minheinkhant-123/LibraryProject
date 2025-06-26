using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryProject.Models
{
    public class BookLoans
    {
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public int MemberId { get; set; }

        [Display(Name = "Loan Date")]
        public DateTime LoanDate { get; set; } = DateTime.Now;

        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        [Display(Name = "Return Date")]
        public DateTime? ReturnDate { get; set; }

        public decimal LateFee { get; set; } = 0;

        public Book Book { get; set; }
        public Members Member { get; set; }
    }
}
