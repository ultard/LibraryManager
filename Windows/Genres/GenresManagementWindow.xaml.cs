using System.Windows;
using LibraryManager.Data;
using LibraryManager.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManager.Windows.Genres;

public partial class GenresManagementWindow
{
    private readonly LibraryDbContext _context;

    public GenresManagementWindow(LibraryDbContext context)
    {
        InitializeComponent();
        _context = context;
        LoadGenres();
    }

    private void LoadGenres()
    {
        var genres = _context.Genres
            .Include(g => g.Books)
            .OrderBy(g => g.Name)
            .ToList();

        GenresDataGrid.ItemsSource = genres;
    }

    private void AddGenre_Click(object sender, RoutedEventArgs e)
    {
        var editWindow = new GenreEditWindow(_context);
        if (editWindow.ShowDialog() == true) LoadGenres();
    }

    private void EditGenre_Click(object sender, RoutedEventArgs e)
    {
        if (GenresDataGrid.SelectedItem is Genre selectedGenre)
        {
            var editWindow = new GenreEditWindow(_context, selectedGenre);
            if (editWindow.ShowDialog() == true) LoadGenres();
        }
        else
        {
            MessageBox.Show("Выберите жанр для редактирования.", "Предупреждение",
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void DeleteGenre_Click(object sender, RoutedEventArgs e)
    {
        if (GenresDataGrid.SelectedItem is Genre selectedGenre)
        {
            if (selectedGenre.Books.Count != 0)
            {
                MessageBox.Show("Нельзя удалить жанр, у которого есть книги.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show($"Удалить жанр {selectedGenre.Name}?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            _context.Genres.Remove(selectedGenre);
            try
            {
                _context.SaveChanges();
                LoadGenres();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        else
        {
            MessageBox.Show("Выберите жанр для удаления.", "Предупреждение",
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}