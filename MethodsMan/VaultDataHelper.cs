using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;

namespace FinalYearProject.MethodsMan
{
    public static class VaultDataHelper
    {
        private static string GetDbPath(string username)
        {
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ScaleLock"
            );
            if (!Directory.Exists(appDataPath))
                Directory.CreateDirectory(appDataPath);

            return Path.Combine(appDataPath, $"{username}_DB.DB");
        }

        // Ensure DB and table exist
        private static void EnsureDatabase(string username)
        {
            string dbFile = GetDbPath(username);

            bool newDb = !File.Exists(dbFile);
            if (newDb)
            {
                SQLiteConnection.CreateFile(dbFile);
            }

            using (var conn = new SQLiteConnection($"Data Source={dbFile};Version=3;"))
            {
                conn.Open();

                string createTable = @"
                    CREATE TABLE IF NOT EXISTS VaultEntries (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Title TEXT NOT NULL,
                        UsernameEncrypted TEXT NOT NULL,
                        PasswordEncrypted TEXT NOT NULL,
                        NotesEncrypted TEXT
                    );";

                using (var cmd = new SQLiteCommand(createTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static ObservableCollection<VaultEntry> RefreshVaultItems(string username)
        {
            EnsureDatabase(username);

            var entries = new ObservableCollection<VaultEntry>();
            string dbFile = GetDbPath(username);

            using (var conn = new SQLiteConnection($"Data Source={dbFile};Version=3;"))
            {
                conn.Open();
                string query = "SELECT Id, Title, UsernameEncrypted, PasswordEncrypted, NotesEncrypted FROM VaultEntries";
                using (var cmd = new SQLiteCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        entries.Add(new VaultEntry
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            UsernameEncrypted = reader.GetString(2),
                            PasswordEncrypted = reader.GetString(3),
                            NotesEncrypted = reader.IsDBNull(4) ? "" : reader.GetString(4)
                        });
                    }
                }
            }

            return entries;
        }

        public static void AddVaultEntry(string username, VaultEntry entry)
        {
            EnsureDatabase(username);

            string dbFile = GetDbPath(username);

            using (var conn = new SQLiteConnection($"Data Source={dbFile};Version=3;"))
            {
                conn.Open();
                string query = "INSERT INTO VaultEntries (Title, UsernameEncrypted, PasswordEncrypted, NotesEncrypted) VALUES (@title, @user, @pass, @notes)";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@title", entry.Title);
                    cmd.Parameters.AddWithValue("@user", entry.UsernameEncrypted);
                    cmd.Parameters.AddWithValue("@pass", entry.PasswordEncrypted);
                    cmd.Parameters.AddWithValue("@notes", entry.NotesEncrypted);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
