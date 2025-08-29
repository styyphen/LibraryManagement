using LibraryManagement.Data;
using Microsoft.EntityFrameworkCore;
using Models;
using Services;

namespace LibraryManagement.Tests;

public class BookServiceTests
{
    private LibraryDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new LibraryDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ValidBook_AddsBook()
    {
        // Arrange
        var db = GetInMemoryDbContext();
        var service = new BookService(db);
        var book = new Book { ISBN = "123", Title = "Test", Author = "Author", PublishedYear = 2020, TotalCopies = 1, CopiesAvailable = 1 };

        // Act
        await service.AddAsync(book);

        // Assert
        Assert.Equal(1, await db.Books.CountAsync());
    }

    [Fact]
    public async Task AddAsync_DuplicateISBN_ThrowsException()
    {
        // Arrange
        var db = GetInMemoryDbContext();
        var service = new BookService(db);
        var book1 = new Book { ISBN = "123", Title = "Test1", Author = "Author", PublishedYear = 2020, TotalCopies = 1, CopiesAvailable = 1 };
        await service.AddAsync(book1);
        var book2 = new Book { ISBN = "123", Title = "Test2", Author = "Author", PublishedYear = 2020, TotalCopies = 1, CopiesAvailable = 1 };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => service.AddAsync(book2));
    }

    [Fact]
    public async Task AddAsync_InvalidCopies_ThrowsException()
    {
        // Arrange
        var db = GetInMemoryDbContext();
        var service = new BookService(db);
        var book = new Book { ISBN = "123", Title = "Test", Author = "Author", PublishedYear = 2020, TotalCopies = 1, CopiesAvailable = 2 };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => service.AddAsync(book));
    }

    [Fact]
    public async Task DeleteAsync_BookWithActiveLoans_ThrowsException()
    {
        // Arrange
        var db = GetInMemoryDbContext();
        var service = new BookService(db);
        var book = new Book { Id = 1, ISBN = "123", Title = "Test", Author = "Author", PublishedYear = 2020, TotalCopies = 1, CopiesAvailable = 0 };
        db.Books.Add(book);
        db.Loans.Add(new Loan { BookId = 1, LenderId = 1, LoanDate = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(14) });
        await db.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => service.DeleteAsync(1));
    }
}
