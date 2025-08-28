using Data;
using Microsoft.EntityFrameworkCore;
using Models;
using Services;
using Xunit;

namespace LibraryManagement.Tests;

public class LoanServiceTests
{
    private LibraryDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new LibraryDbContext(options);
    }

    [Fact]
    public async Task CreateLoanAsync_Valid_DecrementsCopies()
    {
        // Arrange
        var db = GetInMemoryDbContext();
        var book = new Book { Id = 1, ISBN = "123", Title = "Test", Author = "Author", PublishedYear = 2020, TotalCopies = 1, CopiesAvailable = 1 };
        var lender = new Lender { Id = 1, FullName = "Test", Email = "test@example.com" };
        db.Books.Add(book);
        db.Lenders.Add(lender);
        await db.SaveChangesAsync();
        var service = new LoanService(db);

        // Act
        await service.CreateLoanAsync(1, 1);

        // Assert
        Assert.Equal(0, book.CopiesAvailable);
        Assert.Equal(1, await db.Loans.CountAsync());
    }

    [Fact]
    public async Task CreateLoanAsync_NoCopiesAvailable_ThrowsException()
    {
        var db = GetInMemoryDbContext();
        var book = new Book { Id = 1, ISBN = "123", Title = "Test", Author = "Author", PublishedYear = 2020, TotalCopies = 1, CopiesAvailable = 0 };
        var lender = new Lender { Id = 1, FullName = "Test", Email = "test@example.com" };
        db.Books.Add(book);
        db.Lenders.Add(lender);
        await db.SaveChangesAsync();
        var service = new LoanService(db);

        await Assert.ThrowsAsync<Exception>(() => service.CreateLoanAsync(1, 1));
    }

    [Fact]
    public async Task CreateLoanAsync_DuplicateActive_ThrowsException()
    {
        var db = GetInMemoryDbContext();
        var book = new Book { Id = 1, ISBN = "123", Title = "Test", Author = "Author", PublishedYear = 2020, TotalCopies = 2, CopiesAvailable = 2 };
        var lender = new Lender { Id = 1, FullName = "Test", Email = "test@example.com" };
        db.Books.Add(book);
        db.Lenders.Add(lender);
        db.Loans.Add(new Loan { BookId = 1, LenderId = 1, LoanDate = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(14) });
        await db.SaveChangesAsync();
        var service = new LoanService(db);

        await Assert.ThrowsAsync<Exception>(() => service.CreateLoanAsync(1, 1));
    }

    [Fact]
    public async Task CreateLoanAsync_MaxActiveLoans_ThrowsException()
    {
        var db = GetInMemoryDbContext();
        var book1 = new Book { Id = 1, ISBN = "123", Title = "Test1", Author = "Author", PublishedYear = 2020, TotalCopies = 1, CopiesAvailable = 1 };
        var book2 = new Book { Id = 2, ISBN = "456", Title = "Test2", Author = "Author", PublishedYear = 2020, TotalCopies = 1, CopiesAvailable = 1 };
        var book3 = new Book { Id = 3, ISBN = "789", Title = "Test3", Author = "Author", PublishedYear = 2020, TotalCopies = 1, CopiesAvailable = 1 };
        var book4 = new Book { Id = 4, ISBN = "012", Title = "Test4", Author = "Author", PublishedYear = 2020, TotalCopies = 1, CopiesAvailable = 1 };
        var book5 = new Book { Id = 5, ISBN = "345", Title = "Test5", Author = "Author", PublishedYear = 2020, TotalCopies = 1, CopiesAvailable = 1 };
        var book6 = new Book { Id = 6, ISBN = "678", Title = "Test6", Author = "Author", PublishedYear = 2020, TotalCopies = 1, CopiesAvailable = 1 };
        var lender = new Lender { Id = 1, FullName = "Test", Email = "test@example.com" };
        db.Books.AddRange(book1, book2, book3, book4, book5, book6);
        db.Lenders.Add(lender);
        db.Loans.AddRange(
            new Loan { BookId = 1, LenderId = 1, LoanDate = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(14) },
            new Loan { BookId = 2, LenderId = 1, LoanDate = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(14) },
            new Loan { BookId = 3, LenderId = 1, LoanDate = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(14) },
            new Loan { BookId = 4, LenderId = 1, LoanDate = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(14) },
            new Loan { BookId = 5, LenderId = 1, LoanDate = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(14) }
        );
        await db.SaveChangesAsync();
        var service = new LoanService(db);

        await Assert.ThrowsAsync<Exception>(() => service.CreateLoanAsync(6, 1));
    }

    [Fact]
    public async Task CreateLoanAsync_HasOverdue_ThrowsException()
    {
        var db = GetInMemoryDbContext();
        var book1 = new Book { Id = 1, ISBN = "123", Title = "Test1", Author = "Author", PublishedYear = 2020, TotalCopies = 1, CopiesAvailable = 0 };
        var book2 = new Book { Id = 2, ISBN = "456", Title = "Test2", Author = "Author", PublishedYear = 2020, TotalCopies = 1, CopiesAvailable = 1 };
        var lender = new Lender { Id = 1, FullName = "Test", Email = "test@example.com" };
        db.Books.AddRange(book1, book2);
        db.Lenders.Add(lender);
        db.Loans.Add(new Loan { BookId = 1, LenderId = 1, LoanDate = DateTime.UtcNow.AddDays(-20), DueDate = DateTime.UtcNow.AddDays(-6) });
        await db.SaveChangesAsync();
        var service = new LoanService(db);

        await Assert.ThrowsAsync<Exception>(() => service.CreateLoanAsync(2, 1));
    }

    [Fact]
    public async Task ReturnLoanAsync_Valid_IncrementsCopies()
    {
        var db = GetInMemoryDbContext();
        var book = new Book { Id = 1, ISBN = "123", Title = "Test", Author = "Author", PublishedYear = 2020, TotalCopies = 1, CopiesAvailable = 0 };
        var lender = new Lender { Id = 1, FullName = "Test", Email = "test@example.com" };
        var loan = new Loan { Id = 1, BookId = 1, LenderId = 1, LoanDate = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(14) };
        db.Books.Add(book);
        db.Lenders.Add(lender);
        db.Loans.Add(loan);
        await db.SaveChangesAsync();
        var service = new LoanService(db);

        await service.ReturnLoanAsync(1);

        Assert.Equal(1, book.CopiesAvailable);
        Assert.NotNull(loan.ReturnDate);
    }

    [Fact]
    public async Task ReturnLoanAsync_AlreadyReturned_ThrowsException()
    {
        var db = GetInMemoryDbContext();
        var loan = new Loan { Id = 1, BookId = 1, LenderId = 1, LoanDate = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(14), ReturnDate = DateTime.UtcNow };
        db.Loans.Add(loan);
        await db.SaveChangesAsync();
        var service = new LoanService(db);

        await Assert.ThrowsAsync<Exception>(() => service.ReturnLoanAsync(1));
    }
}
