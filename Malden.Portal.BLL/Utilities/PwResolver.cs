using System;
using System.Security.Cryptography;
using System.Text;

namespace Malden.Portal.BLL.Utilities
{
    public static class PasswordResolver
    {
        private const int SALT_BYTE_SIZE = 24;
        private const int HASH_BYTE_SIZE = 24;
        private const int PBKDF2_ITERATIONS = 1000;

        private const int ITERATION_INDEX = 0;
        private const int SALT_INDEX = 1;
        private const int PBKDF2_INDEX = 2;

        private static string ReversePass(string pass)
        {
            var passLength = pass.Length - 1;

            var reversedPass = new StringBuilder();

            for (int i = passLength; i >= 0; i--)
            {
                reversedPass.Append(pass.Substring(i, 1));
            }
            return reversedPass.ToString();
        }

        public static string CreateHash(string password)
        {
            var rngCryptoProvider = new RNGCryptoServiceProvider();

            var salt = new byte[SALT_BYTE_SIZE];
            rngCryptoProvider.GetBytes(salt);

            var reversedPassword = ReversePass(password);

            var hash = GetDerrivedBytes(reversedPassword, salt, PBKDF2_ITERATIONS, HASH_BYTE_SIZE);
            return PBKDF2_ITERATIONS + ":" +
                Convert.ToBase64String(salt) + ":" +
                Convert.ToBase64String(hash);
        }

        public static bool IsValidPass(string password, string hashedPass)
        {
            var reversedPass = ReversePass(password);

            char[] delimiter = { ':' };
            var split = hashedPass.Split(delimiter);
            var iterations = Int32.Parse(split[ITERATION_INDEX]);
            var salt = Convert.FromBase64String(split[SALT_INDEX]);
            var hash = Convert.FromBase64String(split[PBKDF2_INDEX]);

            var testHash = GetDerrivedBytes(reversedPass, salt, iterations, hash.Length);

            return SlowEquals(hash, testHash);
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
                diff |= (uint)(a[i] ^ b[i]);
            return diff == 0;
        }

        private static byte[] GetDerrivedBytes(string password, byte[] salt, int iterations, int outputBytes)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt) { IterationCount = iterations };
            return pbkdf2.GetBytes(outputBytes);
        }

        public static string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}