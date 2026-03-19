using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FinalYearProject.MethodsMan
{
    public static class PasswordGen
    {
        // Thank you, guy on stack overflow!
        // Note that I was instructed to change to "RandomNumberGenerator" because the former was outdated

        public static string GenerateStrongPassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890~@:{}>?<_+";
            StringBuilder res = new StringBuilder(length);
            byte[] buffer = new byte[1];

            using (var rng = RandomNumberGenerator.Create())
            {
                while (res.Length < length)
                {
                    rng.GetBytes(buffer);
                    res.Append(valid[buffer[0] % valid.Length]);
                }
            }

            return res.ToString();
        }
    }
}