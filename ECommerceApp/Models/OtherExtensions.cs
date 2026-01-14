using System;
using System.Linq;

namespace ECommerceApp.Models
{
    /// <summary>
    /// Partial class for Category - Business logic extensions
    /// </summary>
    public partial class Category
    {
        /// <summary>
        /// Gets the product count in this category
        /// </summary>
        public int ProductCount => Products?.Count ?? 0;

        /// <summary>
        /// Gets the active product count
        /// </summary>
        public int ActiveProductCount => Products?.Count(p => p.IsActive) ?? 0;

        /// <summary>
        /// Gets the category display with icon
        /// </summary>
        public string DisplayWithIcon => $"{IconCode ?? ""} {CategoryName}".Trim();

        /// <summary>
        /// Gets a short description (first 50 characters)
        /// </summary>
        public string ShortDescription
        {
            get
            {
                if (string.IsNullOrEmpty(Description)) return "";
                if (Description.Length <= 50) return Description;
                return Description.Substring(0, 50) + "...";
            }
        }
    }

    /// <summary>
    /// Partial class for Review - Business logic extensions
    /// </summary>
    public partial class Review
    {
        /// <summary>
        /// Gets the formatted review date
        /// </summary>
        public string ReviewDateFormatted => ReviewDate.ToString("dd MMM yyyy");

        /// <summary>
        /// Gets the customer name (null-safe)
        /// </summary>
        public string CustomerName => Customer?.FullName ?? "Anonymous";

        /// <summary>
        /// Gets the product name (null-safe)
        /// </summary>
        public string ProductName => Product?.ProductName ?? "Unknown";

        /// <summary>
        /// Gets the rating as stars
        /// </summary>
        public string RatingStars => new string('*', Rating);

        /// <summary>
        /// Gets the rating display
        /// </summary>
        public string RatingDisplay => $"{Rating}/5";

        /// <summary>
        /// Gets the verified purchase badge
        /// </summary>
        public string VerifiedBadge => IsVerifiedPurchase ? "Verified Purchase" : "";

        /// <summary>
        /// Gets the rating color based on score
        /// </summary>
        public string RatingColor
        {
            get
            {
                if (Rating >= 4) return "#4CAF50";
                if (Rating >= 3) return "#FF9800";
                return "#F44336";
            }
        }

        /// <summary>
        /// Gets the short comment (first 100 characters)
        /// </summary>
        public string ShortComment
        {
            get
            {
                if (string.IsNullOrEmpty(Comment)) return "";
                if (Comment.Length <= 100) return Comment;
                return Comment.Substring(0, 100) + "...";
            }
        }
    }

    /// <summary>
    /// Partial class for Tag - Business logic extensions
    /// </summary>
    public partial class Tag
    {
        /// <summary>
        /// Gets the product count with this tag
        /// </summary>
        public int ProductCount => Products?.Count ?? 0;

        /// <summary>
        /// Gets the tag color or default
        /// </summary>
        public string DisplayColor => TagColor ?? "#9E9E9E";

        /// <summary>
        /// Gets foreground color (white or black) based on background
        /// </summary>
        public string ForegroundColor
        {
            get
            {
                if (string.IsNullOrEmpty(TagColor)) return "#FFFFFF";

                // Parse hex color and calculate luminance
                var hex = TagColor.TrimStart('#');
                if (hex.Length != 6) return "#FFFFFF";

                var r = Convert.ToInt32(hex.Substring(0, 2), 16);
                var g = Convert.ToInt32(hex.Substring(2, 2), 16);
                var b = Convert.ToInt32(hex.Substring(4, 2), 16);

                var luminance = (0.299 * r + 0.587 * g + 0.114 * b) / 255;
                return luminance > 0.5 ? "#000000" : "#FFFFFF";
            }
        }
    }

    /// <summary>
    /// Partial class for StoreSetting - Business logic extensions
    /// </summary>
    public partial class StoreSetting
    {
        /// <summary>
        /// Gets the formatted last updated date
        /// </summary>
        public string LastUpdatedFormatted => LastUpdated.ToString("dd MMM yyyy HH:mm");

        /// <summary>
        /// Gets the value as boolean
        /// </summary>
        public bool BooleanValue
        {
            get
            {
                if (SettingType != "Boolean") return false;
                return SettingValue?.ToLower() == "true";
            }
        }

        /// <summary>
        /// Gets the value as integer
        /// </summary>
        public int IntegerValue
        {
            get
            {
                if (SettingType != "Number") return 0;
                int.TryParse(SettingValue, out int result);
                return result;
            }
        }

        /// <summary>
        /// Gets the value as decimal
        /// </summary>
        public decimal DecimalValue
        {
            get
            {
                if (SettingType != "Number") return 0;
                decimal.TryParse(SettingValue, out decimal result);
                return result;
            }
        }

        /// <summary>
        /// Gets the setting type icon
        /// </summary>
        public string TypeIcon
        {
            get
            {
                switch (SettingType)
                {
                    case "Text": return "T";
                    case "Color": return "#";
                    case "Boolean": return "?";
                    case "Number": return "#";
                    default: return "?";
                }
            }
        }

        /// <summary>
        /// Gets the display value (shortened if too long)
        /// </summary>
        public string DisplayValue
        {
            get
            {
                if (string.IsNullOrEmpty(SettingValue)) return "(empty)";
                if (SettingValue.Length <= 30) return SettingValue;
                return SettingValue.Substring(0, 30) + "...";
            }
        }
    }
}
