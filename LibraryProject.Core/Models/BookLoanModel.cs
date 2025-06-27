using System;

namespace LibraryProject.Models
{
    public enum BookLoanStatus
    {
        Active = 1,
        Returned = 2,
        Cancelled = 3,
        Deleted = 0 
    }
    public class BookLoans
    {
        public int Id { get; set; }

        public int MemberId { get; set; }
        public Members? Member { get; set; }

        public int BookId { get; set; }
        public Book? Book { get; set; }

        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }
        public BookLoanStatus Status { get; set; } = BookLoanStatus.Active;
    }
    public class PopularBookViewModel
    {
        public Book Book { get; set; } = null!;
        public int TimesBorrowed { get; set; }
    }

    public class MemberStatsViewModel
    {
        public int TotalMembers { get; set; }
        public int ActiveMembers { get; set; }
        public int SuspendedMembers { get; set; }
        public int ExpiredMembers { get; set; }
    }
}
