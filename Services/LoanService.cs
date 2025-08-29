using LibraryManagement.Data;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Services;

public class LoanService
{
    private readonly LibraryDbContext _db;

    public LoanService(LibraryDbContext db)
    {
        _db = db;
    }

    public async Task<List<Loan>> GetAllAsync()
    {
        return await _db.Loans.Include(l => l.Book).Include(l => l.Lender).ToListAsync();
    }

    public async Task CreateLoanAsync(int bookId, int lenderId)
    {
        var book = await _db.Books.FindAsync(bookId);
        if (book == null || book.CopiesAvailable == 0)
            throw new Exception("Book unavailable.");

        var lender = await _db.Lenders.FindAsync(lenderId);
        if (lender == null)
            throw new Exception("Lender not found.");

        var duplicate = await _db.Loans.AnyAsync(l => l.BookId == bookId && l.LenderId == lenderId && l.ReturnDate == null);
        if (duplicate)
            throw new Exception("Duplicate active loan for this book and lender.");

        var activeLoansCount = await _db.Loans.CountAsync(l => l.LenderId == lenderId && l.ReturnDate == null);
        if (activeLoansCount >= 5)
            throw new Exception("Lender has reached the maximum of 5 active loans.");

        var hasOverdue = await _db.Loans.AnyAsync(l => l.LenderId == lenderId && l.ReturnDate == null && l.DueDate < DateTime.UtcNow);
        if (hasOverdue)
            throw new Exception("Lender has overdue loans and cannot borrow new books.");

        var loan = new Loan
        {
            BookId = bookId,
            LenderId = lenderId,
            LoanDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(14)
        };

        _db.Loans.Add(loan);
        book.CopiesAvailable--;
        await _db.SaveChangesAsync();
    }

    public async Task ReturnLoanAsync(int loanId)
    {
        var loan = await _db.Loans.Include(l => l.Book).FirstOrDefaultAsync(l => l.Id == loanId);
        if (loan == null || loan.ReturnDate != null)
            throw new Exception("Invalid loan or already returned.");

        loan.ReturnDate = DateTime.UtcNow;
        loan.Book!.CopiesAvailable++;
        await _db.SaveChangesAsync();
    }
}
