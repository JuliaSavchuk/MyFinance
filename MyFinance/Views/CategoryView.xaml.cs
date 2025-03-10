using MyFinance.Models;
using MyFinance.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MyFinance.Views
{
    public partial class CategoryView : UserControl
    {
        private readonly CategoryService _categoryService;
        private readonly TransactionService _transactionService;

        public CategoryView()
        {
            InitializeComponent();
            _categoryService = new CategoryService(new AppDbContext());
            _transactionService = new TransactionService(new AppDbContext());
            LoadCategories();
        }

        private async void LoadCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                CategoriesListView.ItemsSource = categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}");
            }
        }

        private async void CategoriesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditButton.IsEnabled = CategoriesListView.SelectedItem != null;
            DeleteButton.IsEnabled = CategoriesListView.SelectedItem != null;

            if (CategoriesListView.SelectedItem is Category selectedCategory)
            {
                try
                {
                    var transactions = await _transactionService.GetTransactionsByBudgetIdAsync(selectedCategory.Transactions.FirstOrDefault()?.BudgetId ?? 0);
                    TransactionsListView.ItemsSource = transactions.Where(t => t.CategoryId == selectedCategory.Id);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading transactions: {ex.Message}");
                }
            }
        }

        private async void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = PromptForInput("New Category", "Enter category name:");
                if (!string.IsNullOrWhiteSpace(name))
                {
                    var category = new Category { Name = name, Color = "#FFFFFF" };
                    _categoryService.AddCategory(category);
                    await _categoryService.SaveChangesAsync();
                    LoadCategories();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding category: {ex.Message}");
            }
        }

        private async void EditCategory_Click(object sender, RoutedEventArgs e)
        {
            if (CategoriesListView.SelectedItem is Category selectedCategory)
            {
                try
                {
                    string newName = PromptForInput("Edit Category", selectedCategory.Name);
                    if (!string.IsNullOrWhiteSpace(newName))
                    {
                        selectedCategory.Name = newName;
                        await _categoryService.UpdateCategoryAsync(selectedCategory);
                        LoadCategories();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error editing category: {ex.Message}");
                }
            }
        }

        private async void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if (CategoriesListView.SelectedItem is Category selectedCategory)
            {
                try
                {
                    if (selectedCategory.Transactions.Any())
                    {
                        MessageBox.Show("Cannot delete category with transactions.");
                        return;
                    }
                    if (MessageBox.Show($"Are you sure you want to delete {selectedCategory.Name}?", "Confirm Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        await _categoryService.DeleteCategoryAsync(selectedCategory.Id);
                        LoadCategories();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting category: {ex.Message}");
                }
            }
        }

        private string PromptForInput(string title, string defaultValue)
        {
            Window prompt = new Window
            {
                Width = 300,
                Height = 150,
                Title = title,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            StackPanel panel = new StackPanel { Margin = new Thickness(10) };
            TextBox textBox = new TextBox { Text = defaultValue, Margin = new Thickness(0, 0, 0, 10) };
            Button okButton = new Button { Content = "OK", Width = 75, Margin = new Thickness(0, 0, 10, 0) };
            Button cancelButton = new Button { Content = "Cancel", Width = 75 };
            StackPanel buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };

            okButton.Click += (s, ev) => { prompt.DialogResult = true; prompt.Close(); };
            cancelButton.Click += (s, ev) => { prompt.Close(); };
            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);
            panel.Children.Add(new TextBlock { Text = $"Enter {title.ToLower().Replace("edit ", "")}:" });
            panel.Children.Add(textBox);
            panel.Children.Add(buttonPanel);
            prompt.Content = panel;

            return prompt.ShowDialog() == true ? textBox.Text : null;
        }
    }

    public class TotalExpenseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ICollection<Transaction> transactions)
            {
                return transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
