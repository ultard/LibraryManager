using System.Collections.ObjectModel;
using System.Windows;
using LibraryManager.Data;
using LibraryManager.Models;
using LibraryManager.Windows.Authors;
using LibraryManager.Windows.Genres;
using Microsoft.EntityFrameworkCore;

namespace LibraryManager;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly LibraryDbContext _context;
    private ObservableCollection<Book> _books = null!;

    public MainWindow()
    {
        InitializeComponent();

        _context = new LibraryDbContext();
        _context.Database.Migrate();

        LoadData();
    }

    private void LoadData()
    {
        _context.Books.Include(b => b.Author).Include(b => b.Genre).Load();
        _books = new ObservableCollection<Book>(_context.Books.ToList());
        BooksDataGrid.ItemsSource = _books;

        AuthorFilterComboBox.ItemsSource = _context.Authors.ToList();
        AuthorFilterComboBox.DisplayMemberPath = "FullName";

        GenreFilterComboBox.ItemsSource = _context.Genres.ToList();
        GenreFilterComboBox.DisplayMemberPath = "Name";
    }

    private void ApplyFilters()
    {
        var query = _context.Books
            .Include(b => b.Author)
            .Include(b => b.Genre)
            .AsQueryable();

        if (AuthorFilterComboBox.SelectedItem is Author selectedAuthor)
            query = query.Where(b => b.AuthorId == selectedAuthor.Id);

        if (GenreFilterComboBox.SelectedItem is Genre selectedGenre)
            query = query.Where(b => b.GenreId == selectedGenre.Id);

        var searchText = TitleSearchTextBox.Text.Trim();
        if (!string.IsNullOrWhiteSpace(searchText))
            query = query.Where(b => b.Title.ToLower().Contains(searchText.ToLower()));
        
        _books.Clear();
        foreach (var book in query.ToList()) _books.Add(book);
    }
    
    private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
    {
        ApplyFilters();
    }
    
    private void ResetFilterButton_Click(object sender, RoutedEventArgs e)
    {
        AuthorFilterComboBox.SelectedIndex = -1;
        GenreFilterComboBox.SelectedIndex = -1;
        TitleSearchTextBox.Clear();
        ApplyFilters();
    }

    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        ApplyFilters();
    }

    private void AddBookButton_Click(object sender, RoutedEventArgs e)
    {
        var addWindow = new BookEditWindow(_context);
        addWindow.ShowDialog();
        LoadData();
    }

    private void EditBookButton_Click(object sender, RoutedEventArgs e)
    {
        if (BooksDataGrid.SelectedItem is not Book selectedBook) return;

        var editWindow = new BookEditWindow(_context, selectedBook);
        editWindow.ShowDialog();
        LoadData();
    }

    private void DeleteBookButton_Click(object sender, RoutedEventArgs e)
    {
        if (BooksDataGrid.SelectedItem is not Book selectedBook) return;

        _context.Books.Remove(selectedBook);
        _context.SaveChanges();
        LoadData();
    }

    private void ManageAuthorsButton_Click(object sender, RoutedEventArgs e)
    {
        var authorsWindow = new AuthorsManagementWindow(_context);
        authorsWindow.ShowDialog();
        LoadData();
    }

    private void ManageGenresButton_Click(object sender, RoutedEventArgs e)
    {
        var genresWindow = new GenresManagementWindow(_context);
        genresWindow.ShowDialog();
        LoadData();
    }
}