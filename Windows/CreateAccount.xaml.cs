using System;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace FinalYearProject.Windows
{
    public partial class CreateAccount : Window
    {
        private string authDbPath;

        public CreateAccount()
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

        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            string username = NewUsernameBox.Text.Trim();
            string password = NewPasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;
            string secretQuestion = (SecretQuestionBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string secretAnswer = SecretAnswerBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword) ||
                password != confirmPassword ||
                password.Length < 6 ||
                string.IsNullOrWhiteSpace(secretAnswer) ||
                secretQuestion == null)
            {
                MessageBox.Show("Please fill in all fields correctly.");
                return;
            }

            using (var connection = new SQLiteConnection($"Data Source={authDbPath}"))
            {
                connection.Open();

                string createTableQuery = @"CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    SecretQuestion TEXT,
                    SecretAnswer TEXT
                );";

                using (var createCmd = new SQLiteCommand(createTableQuery, connection))
                {
                    createCmd.ExecuteNonQuery();
                }

                string insertQuery = @"INSERT INTO Users (Username, PasswordHash, SecretQuestion, SecretAnswer)
                                       VALUES (@Username, @PasswordHash, @SecretQuestion, @SecretAnswer);";

                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@PasswordHash", HashPassword(password));
                    command.Parameters.AddWithValue("@SecretQuestion", secretQuestion);
                    command.Parameters.AddWithValue("@SecretAnswer", secretAnswer);

                    try
                    {
                        command.ExecuteNonQuery();
                        MessageBox.Show("Account created successfully!");
                        this.Close();
                    }
                    catch (SQLiteException ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
