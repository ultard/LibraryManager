using System.Windows;
using LibraryManager.Data;
using LibraryManager.Models;

namespace LibraryManager.Windows.Authors;

public partial class AuthorEditWindow
{
    private readonly Author _author;
    private readonly LibraryDbContext _context;

    public AuthorEditWindow(LibraryDbContext context, Author? author = null)
    {
        InitializeComponent();
        _context = context;
        _author = author ?? new Author();

        if (author != null)
        {
            FirstNameTextBox.Text = author.FirstName;
            LastNameTextBox.Text = author.LastName;
            BirthDatePicker.SelectedDate = author.BirthDate;
            CountryTextBox.Text = author.Country;
        }
        else
        {
            BirthDatePicker.SelectedDate = DateTime.Today.AddYears(-30);
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
            string.IsNullOrWhiteSpace(LastNameTextBox.Text))
        {
            MessageBox.Show("Имя и фамилия обязательны.", "Ошибка валидации",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!BirthDatePicker.SelectedDate.HasValue)
        {
            MessageBox.Show("Укажите дату рождения.", "Ошибка валидации",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _author.FirstName = FirstNameTextBox.Text.Trim();
        _author.LastName = LastNameTextBox.Text.Trim();
        _author.BirthDate = BirthDatePicker.SelectedDate.Value;
        _author.Country = (string.IsNullOrWhiteSpace(CountryTextBox.Text) ? null : CountryTextBox.Text.Trim()) ?? string.Empty;

        if (_author.Id == 0) _context.Authors.Add(_author);

        try
        {
            _context.SaveChanges();
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}