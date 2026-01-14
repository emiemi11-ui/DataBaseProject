using System;
using System.Net.Mail;

namespace ECommerceApp.Models
{
    /// <summary>
    /// Partial class for User - Business logic extensions
    /// Extends the auto-generated User class from EDMX
    /// </summary>
    public partial class User
    {
        /// <summary>
        /// Gets the full name of the user
        /// </summary>
        public string FullName
        {
            get
            {
                var fullName = $"{FirstName ?? ""} {LastName ?? ""}".Trim();
                return string.IsNullOrEmpty(fullName) ? Username : fullName;
            }
        }

        /// <summary>
        /// Gets a display-friendly role name with icon
        /// </summary>
        public string RoleDisplayName
        {
            get
            {
                switch (UserRole)
                {
                    case "StoreOwner":
                        return "Store Owner";
                    case "Customer":
                        return "Customer";
                    case "CustomerService":
                        return "Support Agent";
                    default:
                        return UserRole ?? "Unknown";
                }
            }
        }

        /// <summary>
        /// Gets the role icon
        /// </summary>
        public string RoleIcon
        {
            get
            {
                switch (UserRole)
                {
                    case "StoreOwner":
                        return "M12 2L4 5v6.09c0 5.05 3.41 9.76 8 10.91 4.59-1.15 8-5.86 8-10.91V5l-8-3z"; // Shield icon path
                    case "Customer":
                        return "M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z"; // Person icon path
                    case "CustomerService":
                        return "M20 2H4c-1.1 0-2 .9-2 2v18l4-4h14c1.1 0 2-.9 2-2V4c0-1.1-.9-2-2-2z"; // Chat icon path
                    default:
                        return "M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2z"; // Default circle
                }
            }
        }

        /// <summary>
        /// Gets the status display text
        /// </summary>
        public string StatusDisplay => IsActive ? "Active" : "Inactive";

        /// <summary>
        /// Gets the status color
        /// </summary>
        public string StatusColor => IsActive ? "#4CAF50" : "#F44336";

        /// <summary>
        /// Validates the email format
        /// </summary>
        public bool IsValidEmail()
        {
            if (string.IsNullOrWhiteSpace(Email))
                return false;

            try
            {
                var addr = new MailAddress(Email);
                return addr.Address == Email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets formatted creation date
        /// </summary>
        public string CreatedDateFormatted => CreatedDate.ToString("dd MMM yyyy");

        /// <summary>
        /// Gets the user's initials for avatar display
        /// </summary>
        public string Initials
        {
            get
            {
                var first = !string.IsNullOrEmpty(FirstName) ? FirstName[0].ToString().ToUpper() : "";
                var last = !string.IsNullOrEmpty(LastName) ? LastName[0].ToString().ToUpper() : "";

                if (!string.IsNullOrEmpty(first) && !string.IsNullOrEmpty(last))
                    return first + last;

                if (!string.IsNullOrEmpty(Username) && Username.Length >= 2)
                    return Username.Substring(0, 2).ToUpper();

                return "??";
            }
        }
    }
}
