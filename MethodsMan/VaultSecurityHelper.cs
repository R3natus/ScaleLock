using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Security.Credentials.UI;

namespace FinalYearProject.MethodsMan
{
    public static class VaultSecurityHelper
    {
        // Derive AES key from a passphrase using SHA256
        private static byte[] DeriveKey(string passphrase)
        {
            using (SHA256 sha = SHA256.Create())
            {
                return sha.ComputeHash(Encoding.UTF8.GetBytes(passphrase));
            }
        }

        // Encrypt plain text with AES
        public static string Encrypt(string plainText, string passphrase)
        {
            byte[] key = DeriveKey(passphrase);
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.GenerateIV();

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                {
                    ms.Write(aes.IV, 0, aes.IV.Length); // prepend IV
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        // Decrypt cipher text with AES
        public static string Decrypt(string cipherText, string passphrase)
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText);
            byte[] key = DeriveKey(passphrase);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;

                // Extract IV from the beginning
                byte[] iv = new byte[aes.BlockSize / 8];
                Array.Copy(fullCipher, iv, iv.Length);
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        // Windows Hello authentication before reveal
        public static async Task<bool> AuthenticateWithWindowsHello()
        {
            try
            {
                var result = await UserConsentVerifier.RequestVerificationAsync("Verify to reveal vault data");
                if (result == UserConsentVerificationResult.Verified)
                    return true;

                if (result == UserConsentVerificationResult.DeviceNotPresent)
                    MessageBox.Show("Windows Hello is not available on this device.", "Error");
                else if (result == UserConsentVerificationResult.Canceled)
                    MessageBox.Show("Authentication canceled.", "Info");
                else
                    MessageBox.Show("Authentication failed.", "Error");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during Windows Hello authentication: {ex.Message}", "Error");
            }
            return false;
        }
    }
}
