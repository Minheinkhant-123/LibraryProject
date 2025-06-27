
using LibraryProject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IBookService
{
    Task<List<Book>> GetAllAsync();
    Task<Book> GetByIdAsync(int id);
    Task<bool> CreateAsync(Book book, List<int> authorIds, List<int> genreIds);
    Task<bool> UpdateAsync(Book book, List<int> authorIds, List<int> genreIds);
    Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    Task<List<Author>> GetAllAuthors();
    Task<List<Genre>> GetAllGenres();
}
