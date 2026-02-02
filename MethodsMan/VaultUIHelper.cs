using System.Windows;

namespace FinalYearProject.MethodsMan
{
    public static class VaultUIHelper
    {
        public static void ShowEntryDetails(VaultEntry entry)
        {
            MessageBox.Show(
                $"Title: {entry.Title}\nUsernameHash: {entry.Username}\nPasswordHash: {entry.PasswordHash}\nNotes: {entry.Notes}",
                "Vault Entry"
            );
        }
    }
}
