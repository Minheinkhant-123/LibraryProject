using LibraryProject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryProject.Core.Interfaces
{
    public interface IBookLoanService
    {
        Task<List<BookLoans>> GetAllAsync();
        Task<BookLoans?> GetByIdAsync(int id);
        Task<bool> CreateAsync(int memberId, int bookId);
        Task<bool> UpdateAsync(BookLoans loan);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);

        Task<bool> ReturnBookAsync(int id);
        Task<bool> ExtendLoanAsync(int id);

        Task<int> CountActiveLoansByMemberAsync(int memberId);
        decimal CalculateLateFee(BookLoans loan);
        Task<List<BookLoans>> GetAllWithDetailsAsync();

    }
}
