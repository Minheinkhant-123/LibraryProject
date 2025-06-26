using LibraryProject.Data;
using LibraryProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibraryProject.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // List & Search
        public async Task<IActionResult> Index(string? search)
        {
            var booksQuery = _context.Books
                .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                .Include(b => b.BookGenres).ThenInclude(bg => bg.Genre)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                booksQuery = booksQuery.Where(b =>
                    b.Title.Contains(search) ||
                    b.BookAuthors.Any(ba => ba.Author.Name.Contains(search)) ||
                    b.BookGenres.Any(bg => bg.Genre.Name.Contains(search)));
            }

            var books = await booksQuery.ToListAsync();
            return View(books);
        }

        // Create - GET
        public IActionResult Create()
        {
            ViewBag.Authors = new MultiSelectList(_context.Authors, "Id", "Name");
            ViewBag.Genres = new MultiSelectList(_context.Genres, "Id", "Name");
            return View();
        }

        // Create - POST
        [HttpPost]
        public async Task<IActionResult> Create(Book book, int[] authorIds, int[] genreIds)
        {

            book.BookAuthors = authorIds.Select(id => new BookAuthor { AuthorId = id }).ToList();
            book.BookGenres = genreIds.Select(id => new BookGenre { GenreId = id }).ToList();

            _context.Add(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Edit - GET
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _context.Books
                .Include(b => b.BookAuthors)
                .Include(b => b.BookGenres)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) return NotFound();

            ViewBag.Authors = new MultiSelectList(_context.Authors, "Id", "Name", book.BookAuthors.Select(a => a.AuthorId));
            ViewBag.Genres = new MultiSelectList(_context.Genres, "Id", "Name", book.BookGenres.Select(g => g.GenreId));
            return View(book);
        }

        // Edit - POST
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Book book, int[] authorIds, int[] genreIds)
        {
            if (id != book.Id) return NotFound();

            var existingBook = await _context.Books
                .Include(b => b.BookAuthors)
                .Include(b => b.BookGenres)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (existingBook == null) return NotFound();

            existingBook.Title = book.Title;
            existingBook.ISBN = book.ISBN;
            existingBook.PublicationYear = book.PublicationYear;
            existingBook.Publisher = book.Publisher;
            existingBook.AvailableCopies = book.AvailableCopies;

            existingBook.BookAuthors = authorIds.Select(aid => new BookAuthor { BookId = id, AuthorId = aid }).ToList();
            existingBook.BookGenres = genreIds.Select(gid => new BookGenre { BookId = id, GenreId = gid }).ToList();

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Delete
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
