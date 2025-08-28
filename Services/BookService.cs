using Data;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Services;

public class BookService
{
    private readonly LibraryDbContext _db;

    public BookService(LibraryDbContext db)
    {
        _db = db;
    }

    public async Task<List<Book>> GetAllAsync()
    {
        return await _db.Books.ToListAsync();
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
        return await _db.Books.FindAsync(id);
    }

    public async Task AddAsync(Book book)
    {
        if (await _db.Books.AnyAsync(b => b.ISBN == book.ISBN))
            throw new Exception("ISBN already exists.");

        if (book.CopiesAvailable > book.TotalCopies)
            throw new Exception("Copies Available cannot exceed Total Copies.");

        book.CopiesAvailable = book.TotalCopies; // Default to all available

        _db.Books.Add(book);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Book book)
    {
        var existing = await _db.Books.FirstOrDefaultAsync(b => b.Id == book.Id);
        if (existing == null) throw new Exception("Book not found.");

        if (book.ISBN != existing.ISBN && await _db.Books.AnyAsync(b => b.ISBN == book.ISBN))
            throw new Exception("ISBN already exists.");

        if (book.CopiesAvailable > book.TotalCopies)
            throw new Exception("Copies Available cannot exceed Total Copies.");

        _db.Entry(existing).CurrentValues.SetValues(book);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var book = await _db.Books.FindAsync(id);
        if (book == null) throw new Exception("Book not found.");

        if (await _db.Loans.AnyAsync(l => l.BookId == id && l.ReturnDate == null))
            throw new Exception("Cannot delete book with active loans.");

        _db.Books.Remove(book);
        await _db.SaveChangesAsync();
    }
}
