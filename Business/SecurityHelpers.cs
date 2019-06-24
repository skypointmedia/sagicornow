using System;
using System.Security.Cryptography;

namespace SagicorNow.Business
{
    public class SecurityHelpers
    {
        public static string HashPassword(string password)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(password);
            data = new MD5CryptoServiceProvider().ComputeHash(data);
            return BitConverter.ToString(data).Replace("-", ""); ;
        }

        public static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            if (hashedPassword == null)
            {
                return false;
            }
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            var hash = HashPassword(password);
            return hashedPassword == hash;
        }
    }
}