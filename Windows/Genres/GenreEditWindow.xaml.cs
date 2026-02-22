using System.Diagnostics;
using System.Windows;
using LibraryManager.Data;
using LibraryManager.Models;

namespace LibraryManager.Windows.Genres;

public partial class GenreEditWindow
{
    private readonly LibraryDbContext _context;
    private readonly Genre _genre;

    public GenreEditWindow(LibraryDbContext context, Genre? genre = null)
    {
        InitializeComponent();
        _context = context;
        _genre = genre ?? new Genre();

        if (genre == null) return;
        NameTextBox.Text = genre.Name;
        DescriptionTextBox.Text = genre.Description;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameTextBox.Text))
        {
            MessageBox.Show("Название обязательно.", "Ошибка валидации",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _genre.Name = NameTextBox.Text.Trim();
        _genre.Description =
            (string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ? null : DescriptionTextBox.Text.Trim()) ??
            string.Empty;

        if (_genre.Id == 0) _context.Genres.Add(_genre);

        try
        {
            _context.SaveChanges();
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            var errorMessage = ex.ToString();
            MessageBox.Show($"Ошибка сохранения: {errorMessage}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);

            Debug.WriteLine(errorMessage);
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}