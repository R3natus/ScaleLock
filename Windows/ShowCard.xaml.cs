using System;
using System.Windows;
using System.Windows.Threading;
using FinalYearProject.MethodsMan;

namespace FinalYearProject.Windows
{
    public partial class ShowCard : Window
    {
        private VaultEntry _entry;

        public ShowCard(VaultEntry entry)
        {
            InitializeComponent();
            _entry = entry;
            DataContext = entry;
        }

        private async void ShowUsername_Click(object sender, RoutedEventArgs e)
        {
            if (await VaultSecurityHelper.AuthenticateWithWindowsHello())
            {
                string decrypted = VaultSecurityHelper.Decrypt(_entry.UsernameEncrypted, "ScaleLockSecretKey");
                UsernameBlock.Text = decrypted;
                StartHideTimer(() => UsernameBlock.Text = "••••••••");
            }
        }

        private async void ShowPassword_Click(object sender, RoutedEventArgs e)
        {
            if (await VaultSecurityHelper.AuthenticateWithWindowsHello())
            {
                string decrypted = VaultSecurityHelper.Decrypt(_entry.PasswordEncrypted, "ScaleLockSecretKey");
                PasswordBlock.Text = decrypted;
                StartHideTimer(() => PasswordBlock.Text = "••••••••");
            }
        }

        private async void ShowNotes_Click(object sender, RoutedEventArgs e)
        {
            if (await VaultSecurityHelper.AuthenticateWithWindowsHello())
            {
                string decrypted = VaultSecurityHelper.Decrypt(_entry.NotesEncrypted, "ScaleLockSecretKey");
                NotesBlock.Text = decrypted;
                StartHideTimer(() => NotesBlock.Text = "••••••••");
            }
        }

        private void StartHideTimer(Action hideAction)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Tick += (s, e) =>
            {
                hideAction();
                timer.Stop();
            };
            timer.Start();
        }
    }
}
