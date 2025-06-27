using LibraryProject.Data;
using LibraryProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class BookLoansController : Controller
{
    private readonly ApplicationDbContext _context;

    public BookLoansController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var loans = await _context.BookLoans
            .Include(bl => bl.Book)
            .Include(bl => bl.Member)
            .ToListAsync();
        return View(loans);
    }

    public IActionResult Create()
    {
        ViewBag.Members = new SelectList(_context.Members.Where(m => m.Status == MemberStatus.Active), "Id", "Name");
        ViewBag.Books = new SelectList(_context.Books.Where(b => b.AvailableCopies > 0), "Id", "Title");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookLoans loan)
    {
        var member = await _context.Members.FindAsync(loan.MemberId);
        var book = await _context.Books.FindAsync(loan.BookId);

        // ✅ Validations
        if (member == null || member.Status != MemberStatus.Active)
        {
            ModelState.AddModelError("", "Member is not active.");
        }
        if (book == null || book.AvailableCopies < 1)
        {
            ModelState.AddModelError("", "No copies available.");
        }

        var activeLoans = await _context.BookLoans.CountAsync(bl => bl.MemberId == loan.MemberId && bl.ReturnDate == null);
        if (activeLoans >= 5)
        {
            ModelState.AddModelError("", "Member has already borrowed the maximum number of books (5).");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Members = new SelectList(_context.Members.Where(m => m.Status == MemberStatus.Active), "Id", "Name", loan.MemberId);
            ViewBag.Books = new SelectList(_context.Books.Where(b => b.AvailableCopies > 0), "Id", "Title", loan.BookId);
            return View(loan);
        }

        loan.LoanDate = DateTime.Now;
        loan.DueDate = loan.LoanDate.AddDays(14);

        book.AvailableCopies -= 1;
        _context.BookLoans.Add(loan);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Return(int id)
    {
        var loan = await _context.BookLoans.Include(b => b.Book).FirstOrDefaultAsync(x => x.Id == id);
        if (loan == null || loan.ReturnDate != null) return NotFound();

        loan.ReturnDate = DateTime.Now;

        // ✅ Calculate Late Fee
        var overdueDays = (loan.ReturnDate.Value - loan.DueDate).Days;
        if (overdueDays > 0)
        {
            loan.LateFee = overdueDays * 0.50m;
        }

        loan.Book.AvailableCopies += 1;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Extend(int id)
    {
        var loan = await _context.BookLoans.FindAsync(id);
        if (loan == null || loan.ReturnDate != null) return NotFound();

        // Example: Only allow 1 extension and if no reservations
        loan.DueDate = loan.DueDate.AddDays(7);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
