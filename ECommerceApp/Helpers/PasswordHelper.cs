using System;
using System.Security.Cryptography;
using System.Text;

namespace ECommerceApp.Helpers
{
    /// <summary>
    /// Helper class for password hashing and verification using SHA256.
    /// </summary>
    public static class PasswordHelper
    {
        /// <summary>
        /// Computes the SHA256 hash of a password.
        /// </summary>
        /// <param name="password">The plain text password</param>
        /// <returns>The hexadecimal hash string</returns>
        public static string ComputeSHA256Hash(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Verifies if a plain text password matches a hashed password.
        /// </summary>
        /// <param name="password">The plain text password to verify</param>
        /// <param name="hashedPassword">The stored hashed password</param>
        /// <returns>True if the passwords match, false otherwise</returns>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
            {
                return false;
            }

            string computedHash = ComputeSHA256Hash(password);
            return string.Equals(computedHash, hashedPassword, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Validates password strength.
        /// </summary>
        /// <param name="password">The password to validate</param>
        /// <param name="errorMessage">Error message if validation fails</param>
        /// <returns>True if password meets requirements, false otherwise</returns>
        public static bool ValidatePasswordStrength(string password, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrEmpty(password))
            {
                errorMessage = "Password is required.";
                return false;
            }

            if (password.Length < 6)
            {
                errorMessage = "Password must be at least 6 characters long.";
                return false;
            }

            if (password.Length > 50)
            {
                errorMessage = "Password cannot exceed 50 characters.";
                return false;
            }

            return true;
        }
    }
}
