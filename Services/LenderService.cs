using LibraryManagement.Data;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Services;

public class LenderService
{
    private readonly LibraryDbContext _db;

    public LenderService(LibraryDbContext db)
    {
        _db = db;
    }

    public async Task<List<Lender>> GetAllAsync()
    {
        return await _db.Lenders.ToListAsync();
    }

    public async Task<Lender?> GetByIdAsync(int id)
    {
        return await _db.Lenders.FindAsync(id);
    }

    public async Task AddAsync(Lender lender)
    {
        _db.Lenders.Add(lender);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Lender lender)
    {
        var existing = await _db.Lenders.FindAsync(lender.Id);
        if (existing == null) throw new Exception("Lender not found.");

        _db.Entry(existing).CurrentValues.SetValues(lender);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var lender = await _db.Lenders.FindAsync(id);
        if (lender == null) throw new Exception("Lender not found.");

        if (await _db.Loans.AnyAsync(l => l.LenderId == id && l.ReturnDate == null))
            throw new Exception("Cannot delete lender with active loans.");

        _db.Lenders.Remove(lender);
        await _db.SaveChangesAsync();
    }
}
