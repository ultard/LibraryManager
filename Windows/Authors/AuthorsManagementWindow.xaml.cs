using System.Windows;
using LibraryManager.Data;
using LibraryManager.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManager.Windows.Authors;

public partial class AuthorsManagementWindow
{
    private readonly LibraryDbContext _context;

    public AuthorsManagementWindow(LibraryDbContext context)
    {
        _context = context;
        InitializeComponent();
        LoadAuthors();
    }

    private void LoadAuthors()
    {
        var authors = _context.Authors
            .Include(a => a.Books)
            .OrderBy(a => a.LastName)
            .ThenBy(a => a.FirstName)
            .ToList();

        AuthorsDataGrid.ItemsSource = authors;
    }

    private void AddAuthor_Click(object sender, RoutedEventArgs e)
    {
        var editWindow = new AuthorEditWindow(_context);
        if (editWindow.ShowDialog() == true) LoadAuthors();
    }

    private void EditAuthor_Click(object sender, RoutedEventArgs e)
    {
        if (AuthorsDataGrid.SelectedItem is Author selectedAuthor)
        {
            var editWindow = new AuthorEditWindow(_context, selectedAuthor);
            if (editWindow.ShowDialog() == true) LoadAuthors();
        }
        else
        {
            MessageBox.Show("Выберите автора для редактирования.", "Предупреждение",
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void DeleteAuthor_Click(object sender, RoutedEventArgs e)
    {
        if (AuthorsDataGrid.SelectedItem is Author selectedAuthor)
        {
            if (selectedAuthor.Books.Count != 0)
            {
                MessageBox.Show("Нельзя удалить автора, у которого есть книги.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show($"Удалить автора {selectedAuthor.FullName}?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            _context.Authors.Remove(selectedAuthor);
            try
            {
                _context.SaveChanges();
                LoadAuthors();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        else
        {
            MessageBox.Show("Выберите автора для удаления.", "Предупреждение",
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}