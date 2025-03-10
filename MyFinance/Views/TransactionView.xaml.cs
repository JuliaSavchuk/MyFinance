using MyFinance.Models;
using MyFinance.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MyFinance.Views
{
    public partial class TransactionView : UserControl
    {
        private readonly TransactionService _transactionService;
        private readonly BudgetService _budgetService;
        private readonly CategoryService _categoryService;
        private User _user;
        private List<Transaction> _currentTransactions;

        public TransactionView()
        {
            InitializeComponent();
            _transactionService = new TransactionService(new AppDbContext());
            _budgetService = new BudgetService(new AppDbContext());
            _categoryService = new CategoryService(new AppDbContext());
        }

        public void SetUser(User user)
        {
            _user = user;
            LoadBudgets();
        }

        private async void LoadBudgets()
        {
            try
            {
                var budgets = await _budgetService.GetBudgetsByUserIdAsync(_user.Id);
                BudgetComboBox.ItemsSource = budgets;
                BudgetComboBox.DisplayMemberPath = "Name";
                if (budgets.Count > 0) BudgetComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading budgets: {ex.Message}");
            }
        }

        private async void BudgetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (BudgetComboBox.SelectedItem is Budget selectedBudget)
                {
                    _currentTransactions = await _transactionService.GetTransactionsByBudgetIdAsync(selectedBudget.Id);
                    ApplySorting();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading transactions: {ex.Message}");
            }
        }

        private void TransactionsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditButton.IsEnabled = TransactionsListView.SelectedItem != null;
            DeleteButton.IsEnabled = TransactionsListView.SelectedItem != null;
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplySorting();
        }

        private void ApplySorting()
        {
            if (_currentTransactions == null) return;

            var selectedSort = (SortComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            IEnumerable<Transaction> sortedTransactions = _currentTransactions;

            switch (selectedSort)
            {
                case "Date Ascending":
                    sortedTransactions = _currentTransactions.OrderBy(t => t.Date);
                    break;
                case "Date Descending":
                    sortedTransactions = _currentTransactions.OrderByDescending(t => t.Date);
                    break;
                case "Amount Ascending":
                    sortedTransactions = _currentTransactions.OrderBy(t => t.Amount);
                    break;
                case "Amount Descending":
                    sortedTransactions = _currentTransactions.OrderByDescending(t => t.Amount);
                    break;
                case "Description Ascending":
                    sortedTransactions = _currentTransactions.OrderBy(t => t.Description);
                    break;
            }

            TransactionsListView.ItemsSource = sortedTransactions.ToList();
        }

        private void AddTransaction_Click(object sender, RoutedEventArgs e)
        {
            if (BudgetComboBox.SelectedItem is Budget selectedBudget)
            {
                var window = new TransactionEditWindow(selectedBudget.Id);
                if (window.ShowDialog() == true)
                {
                    BudgetComboBox_SelectionChanged(null, null);
                }
            }
            else
            {
                MessageBox.Show("Please select a budget first.");
            }
        }

        private void EditTransaction_Click(object sender, RoutedEventArgs e)
        {
            if (TransactionsListView.SelectedItem is Transaction selectedTransaction)
            {
                var window = new TransactionEditWindow(selectedTransaction.BudgetId, selectedTransaction);
                if (window.ShowDialog() == true)
                {
                    BudgetComboBox_SelectionChanged(null, null);
                }
            }
        }

        private async void DeleteTransaction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TransactionsListView.SelectedItem is Transaction selectedTransaction)
                {
                    if (MessageBox.Show($"Are you sure you want to delete {selectedTransaction.Description}?", "Confirm Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        await _transactionService.DeleteTransactionAsync(selectedTransaction.Id);
                        BudgetComboBox_SelectionChanged(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting transaction: {ex.Message}");
            }
        }
    }
}