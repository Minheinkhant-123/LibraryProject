using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LibraryProject.Core.Interfaces;
using LibraryProject.Models;


public class BookLoansController : Controller
{
    private readonly IBookLoanService _bookLoanService;
    private readonly IMemberService _memberService;
    private readonly IBookService _bookService;

    public BookLoansController(
        IBookLoanService bookLoanService,
        IMemberService memberService,
        IBookService bookService)
    {
        _bookLoanService = bookLoanService;
        _memberService = memberService;
        _bookService = bookService;
    }

    public async Task<IActionResult> Index()
    {
        var loans = await _bookLoanService.GetAllAsync();
        ViewBag.CalculateLateFee = new Func<BookLoans, decimal>(loan => _bookLoanService.CalculateLateFee(loan));
        return View(loans);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateDropdowns();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(int memberId, int bookId)
    {
        var errors = new List<string>();

        var member = await _memberService.GetByIdAsync(memberId);
        var book = await _bookService.GetByIdAsync(bookId);

        if (member == null)
            errors.Add("Selected member does not exist.");

        if (book == null)
            errors.Add("Selected book does not exist.");

        if (member != null && member.Status != MemberStatus.Active)
            errors.Add("Member is not active and cannot borrow books.");

        if (book != null && book.AvailableCopies <= 0)
            errors.Add("No copies of the selected book are available.");

        var currentLoansCount = await _bookLoanService.CountActiveLoansByMemberAsync(memberId);
        if (currentLoansCount >= 5)
            errors.Add("Member has reached maximum allowed loans (5 books).");

        if (errors.Count > 0)
        {
            ViewBag.Errors = errors;
            await PopulateDropdowns();
            ViewBag.SelectedMemberId = memberId;
            ViewBag.SelectedBookId = bookId;
            return View();
        }

        await _bookLoanService.CreateAsync(memberId, bookId);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var loan = await _bookLoanService.GetByIdAsync(id);
        if (loan == null)
            return NotFound();

        await PopulateDropdowns();
        return View(loan);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(BookLoans loan)
    {
        var errors = new List<string>();

        var member = await _memberService.GetByIdAsync(loan.MemberId);
        var book = await _bookService.GetByIdAsync(loan.BookId);

        if (member == null)
            errors.Add("Member does not exist.");

        if (book == null)
            errors.Add("Book does not exist.");

        if (member != null && member.Status != MemberStatus.Active)
            errors.Add("Member is not active.");

        if (book != null && book.AvailableCopies <= 0)
            errors.Add("No copies available.");

        if (errors.Count > 0)
        {
            ViewBag.Errors = errors;
            await PopulateDropdowns();
            return View(loan);
        }

        var updated = await _bookLoanService.UpdateAsync(loan);
        if (!updated)
            return NotFound();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var loan = await _bookLoanService.GetByIdAsync(id);
        if (loan == null)
            return NotFound();
        return View(loan);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _bookLoanService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Return(int id)
    {
        var success = await _bookLoanService.ReturnBookAsync(id);
        if (!success)
            TempData["Error"] = "Failed to return book.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Extend(int id)
    {
        var success = await _bookLoanService.ExtendLoanAsync(id);
        if (!success)
            TempData["Error"] = "Cannot extend loan: reservation exists or loan invalid.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdowns()
    {
        var members = await _memberService.GetAllAsync();
        ViewBag.Members = new SelectList(members, "Id", "Name");

        var books = await _bookService.GetAllAsync();
        ViewBag.Books = new SelectList(books, "Id", "Title");
    }
}
