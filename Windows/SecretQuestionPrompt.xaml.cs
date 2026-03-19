using System;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace FinalYearProject.Windows
{
    public partial class SecretQuestionPrompt : Window
    {
        private string _username;
        private string _secretQuestion;
        private string authDbPath;

        public SecretQuestionPrompt(string username, string secretQuestion)
        {
            InitializeComponent();

            _username = username;
            _secretQuestion = secretQuestion;

            SecretQuestionText.Text = secretQuestion;

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string scaleLockDir = Path.Combine(appDataPath, "ScaleLock");
            authDbPath = Path.Combine(scaleLockDir, "ScaleLockAuth.db");
        }

        private string Hash(string input)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToBase64String(bytes);
            }
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            string answer = AnswerBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(answer))
            {
                MessageBox.Show("Please enter an answer!");
                return;
            }

            using (var connection = new SQLiteConnection($"Data Source={authDbPath}"))
            {
                connection.Open();

                string query = "SELECT SecretAnswer FROM Users WHERE Username = @Username";

                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", _username);

                    var result = cmd.ExecuteScalar();

                    if (result == null)
                    {
                        MessageBox.Show("Please fill in all fields as instructed!");
                        return;
                    }

                    string storedHash = result.ToString();
                    string enteredHash = Hash(answer);

                    if (storedHash != enteredHash)
                    {
                        MessageBox.Show("Incorrect answer!");
                        return;
                    }

                    // Correct answer → open reset password window
                    ResetPassword rp = new ResetPassword(_username);
                    rp.Show();
                    this.Close();
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
