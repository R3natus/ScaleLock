using System;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using FinalYearProject.MethodsMan;

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
                                            UsernameEncrypted TEXT NOT NULL,
                                            PasswordEncrypted TEXT NOT NULL,
                                            NotesEncrypted TEXT
                                        );";
                    using (var cmd = new SQLiteCommand(tableCmd, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            // Encrypt values before saving
            string encryptedUsername = VaultSecurityHelper.Encrypt(username, "ScaleLockSecretKey");
            string encryptedPassword = VaultSecurityHelper.Encrypt(password, "ScaleLockSecretKey");
            string encryptedNotes = VaultSecurityHelper.Encrypt(notes, "ScaleLockSecretKey");

            using (var conn = new SQLiteConnection($"Data Source={dbFile};Version=3;"))
            {
                conn.Open();
                string insertCmd = "INSERT INTO VaultEntries (Title, UsernameEncrypted, PasswordEncrypted, NotesEncrypted) VALUES (@title, @usernameEnc, @passwordEnc, @notesEnc)";
                using (var cmd = new SQLiteCommand(insertCmd, conn))
                {
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@usernameEnc", encryptedUsername);
                    cmd.Parameters.AddWithValue("@passwordEnc", encryptedPassword);
                    cmd.Parameters.AddWithValue("@notesEnc", encryptedNotes);
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Entry saved successfully!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);

            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
