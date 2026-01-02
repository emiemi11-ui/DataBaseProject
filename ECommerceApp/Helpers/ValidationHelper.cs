using System;
using System.Text.RegularExpressions;

namespace ECommerceApp.Helpers
{
    /// <summary>
    /// Helper class for input validation.
    /// </summary>
    public static class ValidationHelper
    {
        private static readonly Regex EmailRegex = new Regex(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex UsernameRegex = new Regex(
            @"^[a-zA-Z0-9_]{3,50}$",
            RegexOptions.Compiled);

        /// <summary>
        /// Validates an email address format.
        /// </summary>
        /// <param name="email">The email address to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            return EmailRegex.IsMatch(email);
        }

        /// <summary>
        /// Validates a username format (alphanumeric and underscore, 3-50 chars).
        /// </summary>
        /// <param name="username">The username to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool IsValidUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return false;
            }

            return UsernameRegex.IsMatch(username);
        }

        /// <summary>
        /// Validates a price value.
        /// </summary>
        /// <param name="price">The price to validate</param>
        /// <returns>True if valid (non-negative), false otherwise</returns>
        public static bool IsValidPrice(decimal price)
        {
            return price >= 0;
        }

        /// <summary>
        /// Validates a quantity value.
        /// </summary>
        /// <param name="quantity">The quantity to validate</param>
        /// <returns>True if valid (positive), false otherwise</returns>
        public static bool IsValidQuantity(int quantity)
        {
            return quantity > 0;
        }

        /// <summary>
        /// Validates a stock quantity value.
        /// </summary>
        /// <param name="quantity">The stock quantity to validate</param>
        /// <returns>True if valid (non-negative), false otherwise</returns>
        public static bool IsValidStockQuantity(int quantity)
        {
            return quantity >= 0;
        }

        /// <summary>
        /// Validates a rating value (1-5).
        /// </summary>
        /// <param name="rating">The rating to validate</param>
        /// <returns>True if valid (1-5), false otherwise</returns>
        public static bool IsValidRating(int rating)
        {
            return rating >= 1 && rating <= 5;
        }

        /// <summary>
        /// Validates a required string field.
        /// </summary>
        /// <param name="value">The string value to validate</param>
        /// <param name="fieldName">The name of the field for error messages</param>
        /// <param name="maxLength">Maximum allowed length (optional)</param>
        /// <param name="errorMessage">Error message if validation fails</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool ValidateRequiredString(string value, string fieldName, int maxLength, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(value))
            {
                errorMessage = $"{fieldName} is required.";
                return false;
            }

            if (maxLength > 0 && value.Length > maxLength)
            {
                errorMessage = $"{fieldName} cannot exceed {maxLength} characters.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates a decimal value is within a range.
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <param name="min">Minimum allowed value</param>
        /// <param name="max">Maximum allowed value</param>
        /// <returns>True if within range, false otherwise</returns>
        public static bool IsInRange(decimal value, decimal min, decimal max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// Validates an integer value is within a range.
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <param name="min">Minimum allowed value</param>
        /// <param name="max">Maximum allowed value</param>
        /// <returns>True if within range, false otherwise</returns>
        public static bool IsInRange(int value, int min, int max)
        {
            return value >= min && value <= max;
        }
    }
}
