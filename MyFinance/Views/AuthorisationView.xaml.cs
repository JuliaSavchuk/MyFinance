using MyFinance.Models;
using MyFinance.Services;
using System.Windows;
using System.Windows.Controls;

namespace MyFinance.Views
{
    public partial class AuthorisationView : UserControl
    {
        private readonly UserService _userService;

        public AuthorisationView()
        {
            InitializeComponent();
            _userService = new UserService(new AppDbContext());
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string username = UsernameTextBox.Text;
                string password = PasswordBox.Password;
                var user = await _userService.AuthenticateAsync(username, password);
                if (user != null)
                {
                    var mainWindow = (MainWindow)Application.Current.MainWindow;
                    mainWindow.ShowMainView(user);
                }
                else
                {
                    MessageBox.Show("Invalid username or password");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string username = UsernameTextBox.Text;
                string password = PasswordBox.Password;
                string currency = "UAH"; // За замовчуванням
                var user = await _userService.RegisterAsync(username, password, currency);
                MessageBox.Show("Account created successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}
