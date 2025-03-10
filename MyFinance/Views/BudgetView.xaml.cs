using MyFinance.Models;
using MyFinance.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MyFinance.Views
{
    public partial class BudgetView : UserControl
    {
        private readonly BudgetService _budgetService;
        private User _user;
        private ObservableCollection<Budget> _budgets; // Змінили на ObservableCollection

        public BudgetView()
        {
            InitializeComponent();
            _budgetService = new BudgetService(new AppDbContext());
            _budgets = new ObservableCollection<Budget>();
            BudgetsListView.ItemsSource = _budgets; // Прив'язка до ObservableCollection
            Resources.Add("BalanceConverter", new BalanceConverter());
        }

        public void SetUser(User user)
        {
            _user = user;
            DataContext = _user;
            LoadBudgets();
        }

        private async void LoadBudgets()
        {
            try
            {
                var budgets = await _budgetService.GetBudgetsByUserIdAsync(_user.Id);
                _budgets.Clear(); // Очищаємо колекцію
                foreach (var budget in budgets)
                {
                    _budgets.Add(budget); // Додаємо елементи до ObservableCollection
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading budgets: {ex.Message}");
            }
        }

        private async void AddBudget_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var newBudget = await _budgetService.CreateBudgetAsync(_user.Id, "New Budget", 0);
                _budgets.Add(newBudget); // Додаємо новий бюджет до колекції
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding budget: {ex.Message}");
            }
        }

        private void BudgetsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditButton.IsEnabled = BudgetsListView.SelectedItem != null;
            DeleteButton.IsEnabled = BudgetsListView.SelectedItem != null;
        }

        private async void EditBudget_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BudgetsListView.SelectedItem is Budget selectedBudget)
                {
                    string newName = PromptForInput("Edit Budget Name", selectedBudget.Name);
                    if (newName != null)
                    {
                        decimal newLimit;
                        string limitInput = PromptForInput("Edit Budget Limit", selectedBudget.Limit.ToString());
                        if (decimal.TryParse(limitInput, out newLimit))
                        {
                            selectedBudget.Name = newName;
                            selectedBudget.Limit = newLimit;
                            await _budgetService.UpdateBudgetAsync(selectedBudget);
                            // Оновлення відбудеться автоматично через прив'язку
                            BudgetsListView.Items.Refresh(); // Примусове оновлення для відображення змін
                        }
                        else
                        {
                            MessageBox.Show("Invalid limit value.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing budget: {ex.Message}");
            }
        }

        private async void DeleteBudget_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BudgetsListView.SelectedItem is Budget selectedBudget)
                {
                    if (MessageBox.Show($"Are you sure you want to delete {selectedBudget.Name}?", "Confirm Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        await _budgetService.DeleteBudgetAsync(selectedBudget.Id);
                        _budgets.Remove(selectedBudget); // Видаляємо з колекції
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting budget: {ex.Message}");
            }
        }

        private string PromptForInput(string title, string defaultValue)
        {
            // Без змін
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

    // Конвертер без змін
    public class BalanceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is decimal balance && parameter is string currency)
            {
                return $"{balance:F2} {currency}";
            }
            return "0.00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}