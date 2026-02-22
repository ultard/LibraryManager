using System.ComponentModel.DataAnnotations;

namespace LibraryManager.Models;

public class Genre
{
    [Key] public int Id { get; set; }

    [MaxLength(50)] public string Name { get; set; } = null!;
    [MaxLength(500)] public string? Description { get; set; } = string.Empty;

    public ICollection<Book> Books { get; set; } = new List<Book>();
}