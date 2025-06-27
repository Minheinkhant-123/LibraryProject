using LibraryProject.Core.Interfaces;
using LibraryProject.Infrastructure.Data;
using LibraryProject.Models;
using Microsoft.EntityFrameworkCore;

public class MemberService : IMemberService
{
    private readonly ApplicationDbContext _context;

    public MemberService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Members>> GetAllAsync()
    {
        return await _context.Members
            .Where(m => m.Status != MemberStatus.Deleted)
            .ToListAsync();
    }


    public async Task<Members?> GetByIdAsync(int id)
    {
        return await _context.Members.FindAsync(id);
    }

    public async Task CreateAsync(Members member)
    {
        member.RegistrationDate = DateTime.Now;
        member.Status = MemberStatus.Active;
        _context.Members.Add(member);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(Members member)
    {
        var existing = await _context.Members.FindAsync(member.Id);
        if (existing == null) return false;

        existing.Name = member.Name;
        existing.Email = member.Email;
        existing.Phone = member.Phone;
        existing.Address = member.Address;
        existing.Status = member.Status;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
    {
        var hasActiveLoans = await _context.BookLoans
            .AnyAsync(bl => bl.MemberId == id && bl.ReturnDate == null);

        if (hasActiveLoans)
        {
            return (false, "Cannot delete member with active book loans.");
        }

        var member = await _context.Members.FindAsync(id);
        if (member == null)
        {
            return (false, "Member not found.");
        }

        // Soft delete
        member.Status = MemberStatus.Deleted;
        _context.Members.Update(member);
        await _context.SaveChangesAsync();

        return (true, null);
    }
}
