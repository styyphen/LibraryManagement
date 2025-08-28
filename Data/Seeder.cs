using Models;

namespace Data;

public static class Seeder
{
    public static void Seed(LibraryDbContext db)
    {
        if (!db.Books.Any())
        {
            db.Books.AddRange(
                new Book { ISBN = "978-3-16-148410-0", Title = "Book1", Author = "Author1", PublishedYear = 2000, TotalCopies = 5, CopiesAvailable = 5 },
                new Book { ISBN = "978-1-4028-9462-6", Title = "Book2", Author = "Author2", PublishedYear = 2010, TotalCopies = 3, CopiesAvailable = 3 },
                new Book { ISBN = "978-0-545-01022-1", Title = "Book3", Author = "Author3", PublishedYear = 2020, TotalCopies = 2, CopiesAvailable = 0 }
            );
            db.SaveChanges();
        }

        if (!db.Lenders.Any())
        {
            db.Lenders.AddRange(
                new Lender { FullName = "John Doe", Email = "john@example.com" },
                new Lender { FullName = "Jane Smith", Email = "jane@example.com" },
                new Lender { FullName = "Bob Johnson", Email = "bob@example.com" }
            );
            db.SaveChanges();
        }
    }
}
