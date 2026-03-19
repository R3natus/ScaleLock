using System;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace FinalYearProject.Windows
{
    public partial class ResetPassword : Window
    {
        private string _username;
        private string authDbPath;

        public ResetPassword(string username)
        {
            InitializeComponent();
            _username = username;

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string scaleLockDir = Path.Combine(appDataPath, "ScaleLock");
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

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            string newPassword = NewPasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(newPassword) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Please fill in both fields.");
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            if (newPassword.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters.");
                return;
            }

            if (newPassword.Equals(_username, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Username and password cannot be the same.");
                return;
            }

            using (var connection = new SQLiteConnection($"Data Source={authDbPath}"))
            {
                connection.Open();

                string updateQuery = "UPDATE Users SET PasswordHash = @PasswordHash WHERE Username = @Username";

                using (var cmd = new SQLiteCommand(updateQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@PasswordHash", HashPassword(newPassword));
                    cmd.Parameters.AddWithValue("@Username", _username);

                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Password successfully reset! Please try logging in!");
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
