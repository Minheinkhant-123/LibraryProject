using LibraryProject.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryProject.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ 1. Books currently on loan
        public async Task<IActionResult> CurrentLoans()
        {
            var loans = await _context.BookLoans
                .Where(l => l.ReturnDate == null)
                .Include(l => l.Book)
                .Include(l => l.Member)
                .ToListAsync();

            return View(loans);
        }

        // ✅ 2. Overdue books
        public async Task<IActionResult> OverdueBooks()
        {
            var today = DateTime.Now;
            var overdue = await _context.BookLoans
                .Where(l => l.ReturnDate == null && l.DueDate < today)
                .Include(l => l.Book)
                .Include(l => l.Member)
                .ToListAsync();

            return View(overdue);
        }

        // ✅ 3. Popular books
        public async Task<IActionResult> PopularBooks()
        {
            var popular = await _context.BookLoans
                .GroupBy(l => l.BookId)
                .Select(g => new
                {
                    BookId = g.Key,
                    Count = g.Count(),
                    Book = g.First().Book
                })
                .OrderByDescending(g => g.Count)
                .Take(10)
                .ToListAsync();

            return View(popular);
        }

        // ✅ 4. Active members statistics
        public async Task<IActionResult> ActiveMembers()
        {
            var members = await _context.Members
                .Where(m => m.Status == Models.MemberStatus.Active)
                .Select(m => new
                {
                    Member = m,
                    TotalLoans = _context.BookLoans.Count(l => l.MemberId == m.Id)
                })
                .OrderByDescending(m => m.TotalLoans)
                .ToListAsync();

            return View(members);
        }
    }
}
