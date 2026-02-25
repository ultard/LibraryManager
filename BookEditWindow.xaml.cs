using System.Windows;
using System.Windows.Controls;
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
        if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
        {
            MessageBox.Show("Введите название книги!", "Ошибка", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            TitleTextBox.Focus();
            return false;
        }
        
        _book.Title = TitleTextBox.Text; 
        
        if (string.IsNullOrWhiteSpace(ISBNTextBox.Text))
        {
            MessageBox.Show("Введите ISBN!", "Ошибка", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            ISBNTextBox.Focus();
            return false;
        }

        var isbnClean = ISBNTextBox.Text.Trim().Replace("-", "").Replace(" ", "");

        if (!IsValidISBN(isbnClean))
        {
            MessageBox.Show("Неверный формат ISBN!\n" +
                            "• ISBN-10: 10 цифр\n" +
                            "• ISBN-13: 13 цифр", "Ошибка ISBN", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            ISBNTextBox.Focus();
            ISBNTextBox.SelectAll();
            return false;
        }

        _book.ISBN = isbnClean;
        
        var selectedAuthor = (Author?)AuthorComboBox.SelectedItem;
        var selectedGenre = (Genre?)GenreComboBox.SelectedItem;

        if (selectedAuthor is null || selectedGenre is null)
        {
            MessageBox.Show("Выберите автора и жанр!", "Ошибка", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        _book.AuthorId = selectedAuthor.Id;
        _book.GenreId = selectedGenre.Id;
        
        if (string.IsNullOrWhiteSpace(QuantityTextBox.Text) || 
            !int.TryParse(QuantityTextBox.Text, out int quantity) || quantity < 0)
        {
            MessageBox.Show("Введите корректное количество (0 или больше)!", "Ошибка", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            QuantityTextBox.Focus();
            return false;
        }
        
        _book.QuantityInStock = quantity;
        
        if (string.IsNullOrWhiteSpace(PublishYearTextBox.Text) || 
            !int.TryParse(PublishYearTextBox.Text, out int year) || year < 1965 || year > DateTime.Now.Year + 1)
        {
            MessageBox.Show("Введите корректный год издания (1965 - текущий год)!", "Ошибка", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            PublishYearTextBox.Focus();
            return false;
        }
        
        _book.PublishYear = year;

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
    
    private bool IsValidISBN(string isbn)
    {
        if (string.IsNullOrEmpty(isbn)) return false;

        isbn = new string(isbn.Where(char.IsDigit).ToArray());
        
        if (isbn.Length == 10)
        {
            return IsValidISBN10(isbn);
        }

        if (isbn.Length == 13)
        {
            return IsValidISBN13(isbn);
        }

        return false;
    }

    private bool IsValidISBN10(string isbn10)
    {
        var sum = 0;
        for (var i = 0; i < 9; i++)
        {
            sum += int.Parse(isbn10[i].ToString()) * (i + 1);
        }

        var lastChar = isbn10[9];
        var lastDigit = lastChar == 'X' ? 10 : int.Parse(lastChar.ToString());
    
        return sum + lastDigit == 11 * 10;
    }

    private bool IsValidISBN13(string isbn13)
    {
        var sum = 0;
        for (var i = 0; i < 12; i++)
        {
            var digit = int.Parse(isbn13[i].ToString());
            sum += (i % 2 == 0) ? digit : digit * 3;
        }

        var checkDigit = int.Parse(isbn13[12].ToString());
        var total = sum + checkDigit;
    
        return total % 10 == 0;
    }
}