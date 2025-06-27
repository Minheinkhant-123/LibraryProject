using LibraryProject.Infrastructure.Data;
using LibraryProject.Infrastructure.Services;
using LibraryProject.Models;
using Microsoft.EntityFrameworkCore;

public class BookServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly BookService _service;

    public BookServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("BookDb")
            .Options;

        _context = new ApplicationDbContext(options);
        _service = new BookService(_context);
    }

    [Fact]
    public async Task CreateAsync_Should_AddBook()
    {
        var book = new Book { Title = "Book A", ISBN = "111", Publisher = "Pub", PublicationYear = 2020, AvailableCopies = 3 };
        var result = await _service.CreateAsync(book, new List<int>(), new List<int>());
        Assert.True(result);
        Assert.NotNull(await _context.Books.FirstOrDefaultAsync(b => b.Title == "Book A"));
    }

    [Fact]
    public async Task UpdateAsync_Should_ChangeTitle()
    {
        var book = new Book { Title = "Old", ISBN = "222", Publisher = "P", PublicationYear = 2010, AvailableCopies = 1 };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        book.Title = "New";
        await _service.UpdateAsync(book, new List<int>(), new List<int>());

        var updated = await _context.Books.FindAsync(book.Id);
        Assert.Equal("New", updated.Title);
    }

}
