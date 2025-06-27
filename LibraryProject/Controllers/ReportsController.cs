using LibraryProject.Core.Interfaces;
using LibraryProject.Models;
using Microsoft.AspNetCore.Mvc;

public class ReportsController : Controller
{
    private readonly IBookLoanService _bookLoanService;
    private readonly IMemberService _memberService;

    public ReportsController(
        IBookLoanService bookLoanService,
        IMemberService memberService)
    {
        _bookLoanService = bookLoanService;
        _memberService = memberService;
    }

    public async Task<IActionResult> BooksCurrentlyOnLoan()
    {
        var loans = await _bookLoanService.GetAllWithDetailsAsync();
        var BooksCurrentlyOnLoan = loans.Where(l => l.ReturnDate == null).ToList();
        return View(BooksCurrentlyOnLoan);
    }

    public async Task<IActionResult> OverdueBooks()
    {
        var loans = await _bookLoanService.GetAllWithDetailsAsync();
        var overdue = loans.Where(l => l.ReturnDate == null && l.DueDate < DateTime.Now).ToList();
        return View(overdue);
    }

    public async Task<IActionResult> PopularBooks()
    {
        var loans = await _bookLoanService.GetAllWithDetailsAsync();
        var popularBooks = loans
            .GroupBy(l => l.BookId)
            .Select(g => new PopularBookViewModel
            {
                Book = g.First().Book,
                TimesBorrowed = g.Count()
            })
            .OrderByDescending(x => x.TimesBorrowed)
            .Take(10)
            .ToList();

        return View(popularBooks);
    }

    public async Task<IActionResult> ActiveMembersStats()
    {
        var members = await _memberService.GetAllAsync();

        var stats = new MemberStatsViewModel
        {
            TotalMembers = members.Count,
            ActiveMembers = members.Count(m => m.Status == MemberStatus.Active),
            SuspendedMembers = members.Count(m => m.Status == MemberStatus.Suspended),
            ExpiredMembers = members.Count(m => m.Status == MemberStatus.Expired)
        };

        return View(stats);
    }
}
