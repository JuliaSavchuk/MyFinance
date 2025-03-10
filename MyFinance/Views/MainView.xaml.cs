using MyFinance.Models;
using System.Windows;
using System.Windows.Controls;

namespace MyFinance.Views
{
    public partial class MainView : UserControl
    {
        private readonly User _user;

        public MainView(User user)
        {
            InitializeComponent();
            if (user == null)
            {
                MessageBox.Show("No user provided to MainView.");
                return;
            }
            _user = user;
            InitializeTabs();
        }

        private void InitializeTabs()
        {
            var budgetView = new BudgetView();
            budgetView.SetUser(_user);
            MainTabControl.Items.Add(new TabItem { Header = "Budgets", Content = budgetView });

            var transactionView = new TransactionView();
            transactionView.SetUser(_user);
            MainTabControl.Items.Add(new TabItem { Header = "Transactions", Content = transactionView });

            var reportView = new ReportView();
            reportView.SetUser(_user);
            MainTabControl.Items.Add(new TabItem { Header = "Reports", Content = reportView });

            MainTabControl.Items.Add(new TabItem { Header = "Categories", Content = new CategoryView() });
        }
    }
}