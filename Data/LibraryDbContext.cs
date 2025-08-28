using Microsoft.EntityFrameworkCore;
using Models;

namespace Data;

public class LibraryDbContext : DbContext
{
    public DbSet<Book> Books { get; set; } = null!;
    public DbSet<Lender> Lenders { get; set; } = null!;
    public DbSet<Loan> Loans { get; set; } = null!;

    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>().HasIndex(b => b.ISBN).IsUnique();

        modelBuilder.Entity<Loan>()
            .HasOne(l => l.Book)
            .WithMany()
            .HasForeignKey(l => l.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Loan>()
            .HasOne(l => l.Lender)
            .WithMany()
            .HasForeignKey(l => l.LenderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
