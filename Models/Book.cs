using System.ComponentModel.DataAnnotations;

namespace Models;

public class Book
{
    public int Id { get; set; }

    [Required(ErrorMessage = "ISBN is required")]
    [StringLength(13, ErrorMessage = "ISBN must be up to 13 characters")]
    public string ISBN { get; set; } = string.Empty;

    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Author is required")]
    public string Author { get; set; } = string.Empty;

    [Range(1900, 2100, ErrorMessage = "Published Year must be between 1900 and 2100")]
    public int PublishedYear { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Total Copies must be at least 1")]
    public int TotalCopies { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Copies Available must be non-negative")]
    public int CopiesAvailable { get; set; }
}
