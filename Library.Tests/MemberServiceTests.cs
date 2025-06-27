using LibraryProject.Infrastructure.Data;
using LibraryProject.Models;
using Microsoft.EntityFrameworkCore;

public class MemberServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly MemberService _service;

    public MemberServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("MemberDb")
            .Options;

        _context = new ApplicationDbContext(options);
        _service = new MemberService(_context);
    }

    [Fact]
    public async Task CreateAsync_Should_AddMember()
    {
        var member = new Members { Name = "John", Email = "john@example.com", Phone = "123", Address = "Address", RegistrationDate = DateTime.Now };
        var result= await _service.CreateAsync(member);
        if(result== true)
        Assert.True(result);

        Assert.NotNull(await _context.Members.FirstOrDefaultAsync(m => m.Name == "John"));
    }

}
