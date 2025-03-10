using MyFinance.Models;
using MyFinance.Services;
using System;
using System.Collections.Generic;
using System.Windows;

namespace MyFinance.Views
{
    public partial class TransactionEditWindow : Window
    {
        private readonly TransactionService _transactionService;
        private readonly CategoryService _categoryService;
        private readonly int _budgetId;
        private Transaction _transaction;

        public TransactionEditWindow(int budgetId, Transaction transaction = null)
        {
            InitializeComponent();
            _transactionService = new TransactionService(new AppDbContext());
            _categoryService = new CategoryService(new AppDbContext());
            _budgetId = budgetId;
            _transaction = transaction;
            LoadCategories();
            InitializeFields();
        }

        private async void LoadCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                CategoryComboBox.ItemsSource = categories;
                CategoryComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}");
            }
        }

        private void InitializeFields()
        {
            if (_transaction != null)
            {
                DescriptionTextBox.Text = _transaction.Description;
                AmountTextBox.Text = _transaction.Amount.ToString();
                CategoryComboBox.SelectedItem = _transaction.Category;
                IncomeRadio.IsChecked = _transaction.Type == TransactionType.Income;
                ExpenseRadio.IsChecked = _transaction.Type == TransactionType.Expense;
                Title = "Edit Transaction";
            }
            else
            {
                Title = "Add Transaction";
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text) || !decimal.TryParse(AmountTextBox.Text, out decimal amount) || CategoryComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Please fill all fields correctly.");
                    return;
                }

                var type = IncomeRadio.IsChecked == true ? TransactionType.Income : TransactionType.Expense;
                var category = (Category)CategoryComboBox.SelectedItem;

                if (_transaction == null)
                {
                    await _transactionService.CreateTransactionAsync(
                        _budgetId,
                        category.Id,
                        DescriptionTextBox.Text,
                        amount,
                        type,
                        DateTime.Now
                    );
                }
                else
                {
                    _transaction.Description = DescriptionTextBox.Text;
                    _transaction.Amount = amount;
                    _transaction.CategoryId = category.Id;
                    _transaction.Type = type;
                    await _transactionService.UpdateTransactionAsync(_transaction);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving transaction: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
