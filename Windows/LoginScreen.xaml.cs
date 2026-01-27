using System.Windows;

namespace FinalYearProject.Windows
{
    public partial class LoginScreen : Window
    {
        public LoginScreen()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text;
            string password = PasswordBox.Password;

            MessageBox.Show($"Login attempt: {username}");
        }

        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text;
            string password = PasswordBox.Password;

            MessageBox.Show($"Creating account for {username}");
        }
    }
}
