using MyFinance.Models;
using MyFinance.Views;
using System.Windows;

namespace MyFinance
{
    public partial class MainWindow : Window
    {
        private User _currentUser;

        public MainWindow()
        {
            InitializeComponent();
            ShowAuthorisationView();
        }

        private void ShowAuthorisationView()
        {
            TabsPanel.Children.Clear();
            TabsPanel.Children.Add(new AuthorisationView());
            SetButtonsEnabled(false);
            _currentUser = null;
        }

        public void ShowMainView(User user)
        {
            _currentUser = user;
            TabsPanel.Children.Clear();
            TabsPanel.Children.Add(new MainView(user));
            SetButtonsEnabled(true);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            ShowAuthorisationView();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser != null)
            {

                ShowMainView(_currentUser);
            }
            else
            {
                MessageBox.Show("No user is currently logged in.");
            }
        }

        private void SetButtonsEnabled(bool isEnabled)
        {
            RefreshButton.IsEnabled = isEnabled;
            ExitButton.IsEnabled = isEnabled;
        }
    }
}
