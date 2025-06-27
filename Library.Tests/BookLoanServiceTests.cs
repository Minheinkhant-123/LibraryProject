using LibraryProject.Infrastructure.Data;
using LibraryProject.Infrastructure.Services;
using LibraryProject.Models;
using Microsoft.EntityFrameworkCore;

public class BookLoanServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly BookLoanService _service;

    public BookLoanServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("LoanDb")
            .Options;

        _context = new ApplicationDbContext(options);
        _service = new BookLoanService(_context);
    }


    [Fact]
    public async Task ReturnBook_Should_Set_ReturnDate()
    {
        var loan = new BookLoans
        {
            MemberId = 1,
            BookId = 1,
            LoanDate = DateTime.Today.AddDays(-10),
            DueDate = DateTime.Today.AddDays(-5),
            Status = BookLoanStatus.Active
        };
        _context.BookLoans.Add(loan);
        await _context.SaveChangesAsync();

        await _service.ReturnBookAsync(loan.Id);
        var updated = await _context.BookLoans.FindAsync(loan.Id);

        Assert.NotNull(updated.ReturnDate);
    }

    [Fact]
    public async Task DeleteAsync_Should_SoftDelete()
    {
        var loan = new BookLoans { Status = BookLoanStatus.Active };
        _context.BookLoans.Add(loan);
        await _context.SaveChangesAsync();

        await _service.DeleteAsync(loan.Id);
        var deleted = await _context.BookLoans.FindAsync(loan.Id);
        Assert.Equal(BookLoanStatus.Deleted, deleted.Status);
    }
}
