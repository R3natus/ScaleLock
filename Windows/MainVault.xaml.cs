using System.Collections.ObjectModel;
using System.Windows;
using FinalYearProject.MethodsMan;

namespace FinalYearProject.Windows
{
    public partial class MainVault : Window
    {
        public string Username { get; set; }
        public ObservableCollection<VaultEntry> VaultEntries { get; set; }

        public MainVault(string createdUsername)
        {
            InitializeComponent();
            Username = createdUsername;
            DataContext = this;

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Load vault items immediately
            RefreshVaultItems();
        }

        private void AddNew_Click(object sender, RoutedEventArgs e)
        {
            AddEntryWindow addWindow = new AddEntryWindow(Username);
            addWindow.Owner = this;

            if (addWindow.ShowDialog() == true)
            {
                RefreshVaultItems();
            }
        }

        private void RefreshVaultItems()
        {
            VaultEntries = VaultDataHelper.RefreshVaultItems(Username);
            VaultItemsPanel.ItemsSource = VaultEntries;
        }

        private void VaultCard_Click(object sender, RoutedEventArgs e)
        {
            var entry = (sender as System.Windows.Controls.Button)?.DataContext as VaultEntry;
            if (entry != null)
            {
                VaultUIHelper.ShowEntryDetails(entry);
            }
        }

        // Sidebar placeholders
        private void Logins_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Logins placeholder — feature coming soon!", "Info");
        private void Bookmarks_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Bookmarks placeholder — feature coming soon!", "Info");
        private void Safenotes_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Safenotes placeholder — feature coming soon!", "Info");
        private void PasswordGenerator_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Password Generator placeholder — feature coming soon!", "Info");
        private void Help_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Help placeholder — documentation will appear here.", "Info");
        private void TextBox_TextChanged(object sender, RoutedEventArgs e) { }
        private void Profile_Click(object sender, RoutedEventArgs e) => MessageBox.Show($"Profile placeholder — logged in as {Username}", "Info");

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("You have successfully logged out!", "Info");
            LoginScreen loginWindow = new LoginScreen();
            loginWindow.Show();
            Application.Current.MainWindow = loginWindow;
            this.Close();
        }
    }
}
