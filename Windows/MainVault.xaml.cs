using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FinalYearProject.MethodsMan;

namespace FinalYearProject.Windows
{
    public partial class MainVault : Window
    {
        public string Username { get; set; } // Getting username (From logging in)

        private ObservableCollection<VaultEntry> AllVaultEntries { get; set; } // CALL ENTRIES
        public ObservableCollection<VaultEntry> VaultEntries { get; set; } // CALL ENTRIES

        public MainVault(string createdUsername)
        {
            InitializeComponent();
            Username = createdUsername;
            DataContext = this;

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            RefreshVaultItems();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) // Copy and pasted from LoginScreen.xaml.cs for dragging the window around
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e) // Copy and pasted from LoginScreen.xaml.cs for minimizing the window
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e) // Copy and pasted from LoginScreen.xaml.cs for closing the window
        {
            Application.Current.Shutdown();
        }

        private void PasswordGenerator_Click(object sender, RoutedEventArgs e)
        {
            PasswordGenerator generatorWindow = new PasswordGenerator();
            generatorWindow.Owner = this;
            generatorWindow.ShowDialog();
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

        public void RefreshVaultItems()
        {
            AllVaultEntries = VaultDataHelper.RefreshVaultItems(Username);
            VaultEntries = new ObservableCollection<VaultEntry>(AllVaultEntries);

            VaultItemsPanel.ItemsSource = VaultEntries;
        }

        private void VaultCard_Click(object sender, RoutedEventArgs e)
        {
            var entry = (sender as Button)?.DataContext as VaultEntry;
            if (entry != null)
            {
                ShowCard entryWindow = new ShowCard(entry);
                entryWindow.Owner = this;
                entryWindow.ShowDialog();
            }
        }

        private void DeleteCard_Click(object sender, RoutedEventArgs e)
        {
            var entry = (sender as Button)?.DataContext as VaultEntry;
            if (entry == null)
                return;

            if (MessageBox.Show($"Delete '{entry.Title}'?", "Confirm Delete",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                // DELETE FROM DATABASE
                VaultDataHelper.DeleteEntry(entry, Username);

                // REMOVE FROM COLLECTIONS
                AllVaultEntries.Remove(entry);
                VaultEntries.Remove(entry);
            }

            e.Handled = true;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = SearchBox.Text.ToLower();

            VaultEntries.Clear();

            var filtered = AllVaultEntries.Where(item =>
                (item.Title != null && item.Title.ToLower().Contains(query)) ||
                (item.Username != null && item.Username.ToLower().Contains(query)));

            foreach (var item in filtered)
                VaultEntries.Add(item);
        }


        private void Settings_Click(object sender, RoutedEventArgs e) =>
            MessageBox.Show("Bookmarks placeholder — feature coming soon!", "Info");

        private void Help_Click(object sender, RoutedEventArgs e) =>
            MessageBox.Show("Help placeholder — documentation will appear here.", "Info");

        private void Profile_Click(object sender, RoutedEventArgs e) =>
            MessageBox.Show($"Profile placeholder — logged in as {Username}", "Info");

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("You have successfully logged out!", "Info");
            LoginScreen loginWindow = new LoginScreen();
            loginWindow.Show();
            Application.Current.MainWindow = loginWindow;
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
