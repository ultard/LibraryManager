using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManager.Models;

public class Author
{
    [Key] public int Id { get; set; }

    [MaxLength(50)] public string FirstName { get; set; } = null!;
    [MaxLength(50)] public string LastName { get; set; } = null!;
    [MaxLength(100)] public string Country { get; set; } = null!;
    
    public DateTime BirthDate { get; set; }
    
    public ICollection<Book> Books { get; set; } = new List<Book>();

    [NotMapped] public string FullName => $"{FirstName} {LastName}";
}