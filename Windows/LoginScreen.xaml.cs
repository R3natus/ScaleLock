using System;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Windows.Security.Credentials.UI;

namespace FinalYearProject.Windows
{
    public partial class LoginScreen : Window
    {
        private string authDbPath;

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public LoginScreen()
        {
            InitializeComponent();
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string scaleLockDir = Path.Combine(appDataPath, "ScaleLock");
            Directory.CreateDirectory(scaleLockDir);
            authDbPath = Path.Combine(scaleLockDir, "ScaleLockAuth.db");
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password;

            using (var connection = new SQLiteConnection($"Data Source={authDbPath}"))
            {
                connection.Open();
                string query = "SELECT PasswordHash FROM Users WHERE Username = @Username";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    var result = command.ExecuteScalar();

                    if (result != null && result.ToString() == HashPassword(password))
                    {
                        var helloResult = await UserConsentVerifier.RequestVerificationAsync("Verify with Windows Hello to log in");

                        if (helloResult == UserConsentVerificationResult.Verified)
                        {
                            MessageBox.Show("Login successful!");

                            MainVault vaultWindow = new MainVault(username);
                            vaultWindow.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Windows Hello verification aborted!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid username or password.");
                    }
                }
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new CreateAccount();
            registerWindow.ShowDialog();
        }
        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            var forgotWindow = new ForgotPassword();
            forgotWindow.ShowDialog();
        }

    }
}
