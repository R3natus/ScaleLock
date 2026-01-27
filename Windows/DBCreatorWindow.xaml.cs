using System.Windows;
using Microsoft.Data.Sqlite;

namespace FinalYearProject.Windows
{
    public partial class DbCreatorWindow : Window
    {
        private const string ConnectionString = "Data Source=finalyear.db";

        public DbCreatorWindow()
        {
            InitializeComponent();
        }

        private void CreateTable_Click(object sender, RoutedEventArgs e)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
            @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    Salt TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Vault (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    Label TEXT NOT NULL,
                    EncryptedPassword TEXT NOT NULL,
                    CreatedAt TEXT NOT NULL,
                    FOREIGN KEY(UserId) REFERENCES Users(Id)
                );
            ";
            command.ExecuteNonQuery();

            MessageBox.Show("✅ Users and Vault tables created.");
        }
    }
}
