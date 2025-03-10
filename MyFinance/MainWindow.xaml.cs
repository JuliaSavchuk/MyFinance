using MyFinance.Models;
using MyFinance.Views;
using System.Windows;

namespace MyFinance
{
    public partial class MainWindow : Window
    {
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
        }

        public void ShowMainView(User user)
        {
            TabsPanel.Children.Clear();
            TabsPanel.Children.Add(new MainView(user));
            SetButtonsEnabled(true);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            ShowAuthorisationView();
        }

        private void SetButtonsEnabled(bool isEnabled)
        {
            ExitButton.IsEnabled = isEnabled;
        }
    }
}