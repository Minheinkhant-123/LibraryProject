using Microsoft.EntityFrameworkCore;
using LibraryProject.Core.Interfaces;
using LibraryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryProject.Infrastructure.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LibraryProject.Infrastructure.Services
{
    public class BookLoanService : IBookLoanService
    {
        private readonly ApplicationDbContext _context;
        private const decimal LateFeePerDay = 0.5m;
        private const int MaxLoansPerMember = 5;
        private const int LoanDurationDays = 14;
        private const int ExtendDays = 7;

        public BookLoanService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BookLoans>> GetAllAsync()
        {
            return await _context.BookLoans
                .Include(bl => bl.Book)
                .Include(bl => bl.Member)
                .Where(bl => bl.Status != BookLoanStatus.Deleted)
                .ToListAsync();
        }

        public async Task<BookLoans?> GetByIdAsync(int id)
        {
            return await _context.BookLoans
                .Include(bl => bl.Member)
                .Include(bl => bl.Book)
                .FirstOrDefaultAsync(bl => bl.Id == id);
        }

        public async Task<bool> CreateAsync(int memberId, int bookId)
        {
            var member = await _context.Members.FindAsync(memberId);
            var book = await _context.Books.FindAsync(bookId);

            if (member == null || book == null)
                return false;

            if (member.Status != MemberStatus.Active)
                return false;

            if (book.AvailableCopies <= 0)
                return false;

            var activeLoans = await CountActiveLoansByMemberAsync(memberId);
            if (activeLoans >= MaxLoansPerMember)
                return false;

            var loan = new BookLoans
            {
                MemberId = memberId,
                BookId = bookId,
                LoanDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(LoanDurationDays)
            };

            book.AvailableCopies -= 1;

            _context.BookLoans.Add(loan);
            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateAsync(BookLoans loan)
        {
            var existing = await _context.BookLoans.FindAsync(loan.Id);
            if (existing == null)
                return false;

            existing.MemberId = loan.MemberId;
            existing.BookId = loan.BookId;
            existing.LoanDate = loan.LoanDate;
            existing.DueDate = loan.DueDate;
            existing.ReturnDate = loan.ReturnDate;

            _context.BookLoans.Update(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            var loan = await _context.BookLoans.FindAsync(id);
            if (loan == null)
            {
                return (false, "Book loan not found.");
            }

            loan.Status = BookLoanStatus.Deleted;
            _context.BookLoans.Update(loan);
            await _context.SaveChangesAsync();

            return (true, null);
        }


        public async Task<bool> ReturnBookAsync(int id)
        {
            var loan = await _context.BookLoans.FindAsync(id);
            if (loan == null || loan.ReturnDate != null)
                return false;

            loan.ReturnDate = DateTime.Now;

            var book = await _context.Books.FindAsync(loan.BookId);
            if (book != null)
            {
                book.AvailableCopies += 1;
                _context.Books.Update(book);
            }

            _context.BookLoans.Update(loan);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExtendLoanAsync(int id)
        {
            var loan = await _context.BookLoans.FindAsync(id);
            if (loan == null || loan.ReturnDate != null)
                return false;

            // Placeholder for reservation check
            bool hasReservation = await CheckReservationsForBookAsync(loan.BookId);
            if (hasReservation)
                return false;

            loan.DueDate = loan.DueDate.AddDays(ExtendDays);
            _context.BookLoans.Update(loan);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> CountActiveLoansByMemberAsync(int memberId)
        {
            return await _context.BookLoans
                .Where(bl => bl.MemberId == memberId && bl.ReturnDate == null)
                .CountAsync();
        }

        public decimal CalculateLateFee(BookLoans loan)
        {
            if (loan.ReturnDate == null) return 0;

            var overdueDays = (loan.ReturnDate.Value - loan.DueDate).Days;
            if (overdueDays <= 0) return 0;

            return overdueDays * LateFeePerDay;
        }

        private async Task<bool> CheckReservationsForBookAsync(int bookId)
        {
            // Implement actual reservation check here
            return false;
        }
        public async Task<List<BookLoans>> GetAllWithDetailsAsync()
        {
            return await _context.BookLoans
                .Include(bl => bl.Book)
                    .ThenInclude(b => b.BookAuthors)
                        .ThenInclude(ba => ba.Author)
                .Include(bl => bl.Member)
                .ToListAsync();
        }

    }
}
