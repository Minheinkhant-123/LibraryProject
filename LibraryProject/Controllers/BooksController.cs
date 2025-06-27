using LibraryProject.Core.Interfaces;
using LibraryProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryProject.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            var books = await _bookService.GetAllAsync();
            return View(books);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null) return NotFound();
            return View(book);
        }

        // GET: Books/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Authors = new MultiSelectList(await _bookService.GetAllAuthors(), "Id", "Name");
            ViewBag.Genres = new MultiSelectList(await _bookService.GetAllGenres(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Book book, List<int> authorIds, List<int> genreIds)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(book.ISBN))
                errors.Add("ISBN is required.");

            if (string.IsNullOrWhiteSpace(book.Title))
                errors.Add("Title is required.");

            if (book.PublicationYear < 1500 || book.PublicationYear > DateTime.Now.Year)
                errors.Add("Invalid publication year.");

            if (string.IsNullOrWhiteSpace(book.Publisher))
                errors.Add("Publisher is required.");

            if (book.AvailableCopies < 1)
                errors.Add("Available copies must be at least 1.");

            if (errors.Count > 0)
            {
                TempData["Errors"] = errors;
                ViewBag.Authors = new MultiSelectList(await _bookService.GetAllAuthors(), "Id", "Name", authorIds);
                ViewBag.Genres = new MultiSelectList(await _bookService.GetAllGenres(), "Id", "Name", genreIds);
                return View(book);
            }

            await _bookService.CreateAsync(book, authorIds, genreIds);
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null) return NotFound();

            var selectedAuthors = book.BookAuthors.Select(a => a.AuthorId).ToList();
            var selectedGenres = book.BookGenres.Select(g => g.GenreId).ToList();

            ViewBag.Authors = new MultiSelectList(await _bookService.GetAllAuthors(), "Id", "Name", selectedAuthors);
            ViewBag.Genres = new MultiSelectList(await _bookService.GetAllGenres(), "Id", "Name", selectedGenres);

            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Book book, List<int> authorIds, List<int> genreIds)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(book.ISBN))
                errors.Add("ISBN is required.");

            if (string.IsNullOrWhiteSpace(book.Title))
                errors.Add("Title is required.");

            if (book.PublicationYear < 1500 || book.PublicationYear > DateTime.Now.Year)
                errors.Add("Invalid publication year.");

            if (string.IsNullOrWhiteSpace(book.Publisher))
                errors.Add("Publisher is required.");

            if (book.AvailableCopies < 0)
                errors.Add("Available copies must be 0 or greater.");

            if (errors.Count > 0)
            {
                TempData["Errors"] = errors;
                ViewBag.Authors = new MultiSelectList(await _bookService.GetAllAuthors(), "Id", "Name", authorIds);
                ViewBag.Genres = new MultiSelectList(await _bookService.GetAllGenres(), "Id", "Name", genreIds);
                return View(book);
            }

            var success = await _bookService.UpdateAsync(book, authorIds, genreIds);
            if (!success) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null) return NotFound();
            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _bookService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
