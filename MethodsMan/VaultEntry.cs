namespace FinalYearProject.MethodsMan
{
    public class VaultEntry
    {
        public int Id { get; set; }
        public string Title { get; set; }

        // Encrypted fields stored in DB
        public string UsernameEncrypted { get; set; }
        public string PasswordEncrypted { get; set; }
        public string NotesEncrypted { get; set; }

        // Optional: decrypted fields (not stored, just used at runtime)
        public string Username { get; set; }
        public string Password { get; set; }
        public string Notes { get; set; }
    }
}
