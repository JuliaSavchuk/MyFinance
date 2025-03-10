using Microsoft.Win32;
using MyFinance.Models;
using MyFinance.Services;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MyFinance.Views
{
    public partial class ReportView : UserControl
    {
        private readonly TransactionService _transactionService;
        private readonly BudgetService _budgetService;
        private readonly CurrencyService _currencyService;
        private User _user;

        public ReportView()
        {
            InitializeComponent();
            _transactionService = new TransactionService(new AppDbContext());
            _budgetService = new BudgetService(new AppDbContext());
            _currencyService = new CurrencyService(new AppDbContext());
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
                if (BudgetComboBox.SelectedItem is Budget selectedBudget && StartDatePicker.SelectedDate.HasValue && EndDatePicker.SelectedDate.HasValue)
                {
                    var transactions = await _transactionService.GetTransactionsByDateRangeAsync(selectedBudget.Id, StartDatePicker.SelectedDate.Value, EndDatePicker.SelectedDate.Value);
                    ReportListView.ItemsSource = transactions;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading report: {ex.Message}");
            }
        }

        private async void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BudgetComboBox.SelectedItem is Budget selectedBudget && StartDatePicker.SelectedDate.HasValue && EndDatePicker.SelectedDate.HasValue)
                {
                    var transactions = await _transactionService.GetTransactionsByDateRangeAsync(selectedBudget.Id, StartDatePicker.SelectedDate.Value, EndDatePicker.SelectedDate.Value);
                    var sb = new StringBuilder();

                    sb.AppendLine($"Financial Report for {selectedBudget.Name}");
                    sb.AppendLine($"Period: {StartDatePicker.SelectedDate.Value.ToShortDateString()} - {EndDatePicker.SelectedDate.Value.ToShortDateString()}");
                    sb.AppendLine($"Currency: {_user.Currency}");
                    sb.AppendLine("--------------------------------------------------");
                    sb.AppendLine("Date\tDescription\tAmount\tCategory\tType");

                    decimal totalIncome = 0, totalExpense = 0;
                    foreach (var transaction in transactions)
                    {
                        var amountInUserCurrency = transaction.Amount * await _currencyService.GetExchangeRateAsync("UAH", _user.Currency);
                        sb.AppendLine($"{transaction.Date.ToShortDateString()}\t{transaction.Description}\t{amountInUserCurrency}\t{transaction.Category.Name}\t{transaction.Type}");
                        if (transaction.Type == TransactionType.Income) totalIncome += amountInUserCurrency;
                        else totalExpense += amountInUserCurrency;
                    }

                    sb.AppendLine("--------------------------------------------------");
                    sb.AppendLine($"Total Income: {totalIncome} {_user.Currency}");
                    sb.AppendLine($"Total Expense: {totalExpense} {_user.Currency}");
                    sb.AppendLine($"Balance: {totalIncome - totalExpense} {_user.Currency}");

                    var saveFileDialog = new SaveFileDialog { Filter = "Text files (*.txt)|*.txt", DefaultExt = "txt" };
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        File.WriteAllText(saveFileDialog.FileName, sb.ToString());
                        MessageBox.Show("Report saved successfully");
                    }
                }
                else
                {
                    MessageBox.Show("Please select a budget and date range.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}");
            }
        }
    }
}