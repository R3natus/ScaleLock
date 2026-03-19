using System;
using System.Data.SQLite;
using System.IO;
using System.Windows;

namespace FinalYearProject.Windows
{
    public partial class ForgotPassword : Window
    {
        private string authDbPath;

        public ForgotPassword()
        {
            InitializeComponent();

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string scaleLockDir = Path.Combine(appDataPath, "ScaleLock");
            authDbPath = Path.Combine(scaleLockDir, "ScaleLockAuth.db");
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Please enter a username!");
                return;
            }

            using (var connection = new SQLiteConnection($"Data Source={authDbPath}"))
            {
                connection.Open();

                string query = "SELECT SecretQuestion FROM Users WHERE Username = @Username";

                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", username);

                    var result = cmd.ExecuteScalar();

                    if (result == null)
                    {
                        MessageBox.Show("Username not recognised!");
                        return;
                    }

                    string secretQuestion = result.ToString();

                    SecretQuestionPrompt sqp = new SecretQuestionPrompt(username, secretQuestion);
                    sqp.Show();
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
