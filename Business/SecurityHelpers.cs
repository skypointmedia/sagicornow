using System;
using System.Security.Cryptography;

namespace SagicorNow.Business
{
    public class SecurityHelpers
    {
        public static string HashPassword(string password)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(password);
            data = new SHA256Managed().ComputeHash(data);
            return System.Text.Encoding.ASCII.GetString(data);
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