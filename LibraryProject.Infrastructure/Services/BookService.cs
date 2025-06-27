using LibraryProject.Infrastructure.Data;
using LibraryProject.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryProject.Infrastructure.Services
{
    public class BookService : IBookService
    {
        private readonly ApplicationDbContext _context;

        public BookService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Book>> GetAllAsync()
        {
            return await _context.Books
                .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                .Include(b => b.BookGenres).ThenInclude(bg => bg.Genre)
                .Where(b => b.Status == 1)
                .ToListAsync();
        }

        public async Task<Book?> GetByIdAsync(int id)
        {
            var book = await _context.Books
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .Include(b => b.BookGenres)
                    .ThenInclude(bg => bg.Genre)
                .FirstOrDefaultAsync(b => b.Id == id && b.Status == 1); 

            return book;
        }


        public async Task CreateAsync(Book book, List<int> authorIds, List<int> genreIds)
        {
            book.BookAuthors = authorIds
                .Select(authorId => new BookAuthor { AuthorId = authorId })
                .ToList();

            book.BookGenres = genreIds
                .Select(genreId => new BookGenre { GenreId = genreId })
                .ToList();

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Book updatedBook, List<int> authorIds, List<int> genreIds)
        {
            var existingBook = await _context.Books
                .Include(b => b.BookAuthors)
                .Include(b => b.BookGenres)
                .FirstOrDefaultAsync(b => b.Id == updatedBook.Id);

            if (existingBook == null)
                return false;

            existingBook.ISBN = updatedBook.ISBN;
            existingBook.Title = updatedBook.Title;
            existingBook.PublicationYear = updatedBook.PublicationYear;
            existingBook.Publisher = updatedBook.Publisher;
            existingBook.AvailableCopies = updatedBook.AvailableCopies;

            existingBook.BookAuthors.Clear();
            foreach (var authorId in authorIds)
            {
                existingBook.BookAuthors.Add(new BookAuthor
                {
                    BookId = existingBook.Id,
                    AuthorId = authorId
                });
            }

            // Update Genres
            existingBook.BookGenres.Clear();
            foreach (var genreId in genreIds)
            {
                existingBook.BookGenres.Add(new BookGenre
                {
                    BookId = existingBook.Id,
                    GenreId = genreId
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }


public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
{
    var book = await _context.Books.FindAsync(id);
    if (book == null)
    {
        return (false, "Book not found.");
    }

    // Soft delete: update status to 0
    book.Status = 0;
    _context.Books.Update(book);
    await _context.SaveChangesAsync();

    return (true, null);
}

        public async Task<List<Author>> GetAllAuthors()
        {
            return await _context.Authors.ToListAsync();
        }

        public async Task<List<Genre>> GetAllGenres()
        {
            return await _context.Genres.ToListAsync();
        }
    }
}
