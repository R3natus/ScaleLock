using System;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace FinalYearProject.Windows
{
    public partial class LoginScreen : Window
    {
        private string authDbPath;

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

        private void Login_Click(object sender, RoutedEventArgs e)
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
                        MessageBox.Show("Login successful!");
                        // TODO: open vault window here
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
    }
}
