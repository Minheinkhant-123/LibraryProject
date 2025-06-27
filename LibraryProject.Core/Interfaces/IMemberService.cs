using LibraryProject.Models;

namespace LibraryProject.Core.Interfaces
{
    public interface IMemberService
    {
        Task<List<Members>> GetAllAsync();
        Task<Members?> GetByIdAsync(int id);
        Task CreateAsync(Members member);
        Task<bool> UpdateAsync(Members member);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
