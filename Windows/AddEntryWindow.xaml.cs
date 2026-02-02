using System;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace FinalYearProject.Windows
{
    public partial class AddEntryWindow : Window
    {
        private string CurrentUser;

        public AddEntryWindow(string username)
        {
            InitializeComponent();
            CurrentUser = username;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleBox.Text.Trim();
            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password.Trim();
            string notes = NotesBox.Text.Trim();

            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Please enter a title for this entry.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Please enter a username or email.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter a password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Force DB into AppData\Local\ScaleLock
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ScaleLock"
            );

            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            // Use .DB extension instead of .sqlite
            string dbFile = Path.Combine(appDataPath, $"{CurrentUser}_DB.DB");

            if (!File.Exists(dbFile))
            {
                SQLiteConnection.CreateFile(dbFile);

                using (var conn = new SQLiteConnection($"Data Source={dbFile};Version=3;"))
                {
                    conn.Open();
                    string tableCmd = @"CREATE TABLE VaultEntries (
                                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                            Title TEXT NOT NULL,
                                            UsernameHash TEXT NOT NULL,
                                            PasswordHash TEXT NOT NULL,
                                            Notes TEXT
                                        );";
                    using (var cmd = new SQLiteCommand(tableCmd, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            // Hash both username and password
            string hashedUsername = HashString(username);
            string hashedPassword = HashString(password);

            using (var conn = new SQLiteConnection($"Data Source={dbFile};Version=3;"))
            {
                conn.Open();
                string insertCmd = "INSERT INTO VaultEntries (Title, UsernameHash, PasswordHash, Notes) VALUES (@title, @usernameHash, @passwordHash, @notes)";
                using (var cmd = new SQLiteCommand(insertCmd, conn))
                {
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@usernameHash", hashedUsername);
                    cmd.Parameters.AddWithValue("@passwordHash", hashedPassword);
                    cmd.Parameters.AddWithValue("@notes", notes);
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Entry saved successfully!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);

            this.DialogResult = true;
            this.Close();
        }

        private string HashString(string plainText)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(plainText));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
