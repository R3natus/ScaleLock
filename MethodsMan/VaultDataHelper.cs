using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;

namespace FinalYearProject.MethodsMan
{
    public static class VaultDataHelper
    {
        public static ObservableCollection<VaultEntry> RefreshVaultItems(string username)
        {
            var entries = new ObservableCollection<VaultEntry>();

            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ScaleLock"
            );
            string dbFile = Path.Combine(appDataPath, $"{username}_DB.DB");

            if (!File.Exists(dbFile)) return entries;

            using (var conn = new SQLiteConnection($"Data Source={dbFile};Version=3;"))
            {
                conn.Open();
                string query = "SELECT Id, Title, UsernameHash, PasswordHash, Notes FROM VaultEntries";
                using (var cmd = new SQLiteCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        entries.Add(new VaultEntry
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Username = reader.GetString(2),   // hashed username
                            PasswordHash = reader.GetString(3),
                            Notes = reader.IsDBNull(4) ? "" : reader.GetString(4)
                        });
                    }
                }
            }

            return entries;
        }
    }
}
