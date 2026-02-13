using System.Windows;

namespace FinalYearProject.MethodsMan
{
    public static class VaultUIHelper
    {
        public static void ShowEntryDetails(VaultEntry entry)
        {
            MessageBox.Show(
                $"Title: {entry.Title}\n" +
                $"Username (Encrypted): {entry.UsernameEncrypted}\n" +
                $"Password (Encrypted): {entry.PasswordEncrypted}\n" +
                $"Notes (Encrypted): {entry.NotesEncrypted}",
                "Vault Entry"
            );
        }
    }
}
