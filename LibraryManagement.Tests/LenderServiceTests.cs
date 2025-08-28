using Data;
using Microsoft.EntityFrameworkCore;
using Models;
using Services;
using Xunit;

namespace LibraryManagement.Tests;

public class LenderServiceTests
{
    private LibraryDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new LibraryDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ValidLender_AddsLender()
    {
        var db = GetInMemoryDbContext();
        var service = new LenderService(db);
        var lender = new Lender { FullName = "Test", Email = "test@example.com" };

        await service.AddAsync(lender);

        Assert.Equal(1, await db.Lenders.CountAsync());
    }

    [Fact]
    public async Task DeleteAsync_LenderWithActiveLoans_ThrowsException()
    {
        var db = GetInMemoryDbContext();
        var service = new LenderService(db);
        var lender = new Lender { Id = 1, FullName = "Test", Email = "test@example.com" };
        db.Lenders.Add(lender);
        db.Loans.Add(new Loan { BookId = 1, LenderId = 1, LoanDate = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(14) });
        await db.SaveChangesAsync();

        await Assert.ThrowsAsync<Exception>(() => service.DeleteAsync(1));
    }
}
