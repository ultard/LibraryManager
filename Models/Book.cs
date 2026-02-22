using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManager.Models;

public class Book
{
    [Key] public int Id { get; set; }

    [MaxLength(200)] public string Title { get; set; } = null!;

    public int PublishYear { get; set; }

    [MaxLength(20)] public string ISBN { get; set; } = null!;

    public int QuantityInStock { get; set; }

    [ForeignKey(nameof(AuthorId))] public Author Author { get; set; }
    public int AuthorId { get; set; }

    [ForeignKey(nameof(GenreId))] public Genre Genre { get; set; }
    public int GenreId { get; set; }
}