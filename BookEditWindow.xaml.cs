using System.Windows;
using LibraryManager.Data;
using LibraryManager.Models;

namespace LibraryManager;

public partial class BookEditWindow
{
    private readonly Book _book;
    private readonly LibraryDbContext _context;

    public BookEditWindow(LibraryDbContext context, Book? book = null)
    {
        InitializeComponent();
        _context = context;
        _book = book ?? new Book();

        AuthorComboBox.ItemsSource = _context.Authors.ToList();
        GenreComboBox.ItemsSource = _context.Genres.ToList();

        if (book == null) return;

        TitleTextBox.Text = book.Title;
        PublishYearTextBox.Text = book.PublishYear.ToString();
        ISBNTextBox.Text = book.ISBN;
        QuantityTextBox.Text = book.QuantityInStock.ToString();
        AuthorComboBox.SelectedItem = book.Author;
        GenreComboBox.SelectedItem = book.Genre;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        _book.Title = TitleTextBox.Text;
        _book.PublishYear = int.Parse(PublishYearTextBox.Text);
        _book.ISBN = ISBNTextBox.Text;
        _book.QuantityInStock = int.Parse(QuantityTextBox.Text);
        _book.AuthorId = ((Author)AuthorComboBox.SelectedItem).Id;
        _book.GenreId = ((Genre)GenreComboBox.SelectedItem).Id;

        if (_book.Id == 0)
            _context.Books.Add(_book);
        else
            _context.Books.Update(_book);

        _context.SaveChanges();
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}