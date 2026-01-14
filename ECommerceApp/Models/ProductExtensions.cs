using System;
using System.Linq;

namespace ECommerceApp.Models
{
    /// <summary>
    /// Partial class for Product - Business logic extensions
    /// Extends the auto-generated Product class from EDMX
    /// </summary>
    public partial class Product
    {
        /// <summary>
        /// Gets the formatted price with currency symbol
        /// </summary>
        public string PriceFormatted => $"{Price:N2} RON";

        /// <summary>
        /// Gets the price formatted without currency for input fields
        /// </summary>
        public string PriceValue => Price.ToString("F2");

        /// <summary>
        /// Checks if product has low stock
        /// </summary>
        public bool IsLowStock
        {
            get
            {
                if (Inventory == null) return false;
                return Inventory.StockQuantity < Inventory.MinimumStock;
            }
        }

        /// <summary>
        /// Checks if product is out of stock
        /// </summary>
        public bool IsOutOfStock
        {
            get
            {
                if (Inventory == null) return true;
                return Inventory.StockQuantity <= 0;
            }
        }

        /// <summary>
        /// Gets the stock status text
        /// </summary>
        public string StockStatus
        {
            get
            {
                if (Inventory == null) return "Unknown";
                if (Inventory.StockQuantity <= 0) return "Out of Stock";
                if (IsLowStock) return "Low Stock";
                return "In Stock";
            }
        }

        /// <summary>
        /// Gets the stock status color
        /// </summary>
        public string StockStatusColor
        {
            get
            {
                if (Inventory == null) return "#9E9E9E";
                if (Inventory.StockQuantity <= 0) return "#F44336";
                if (IsLowStock) return "#FF9800";
                return "#4CAF50";
            }
        }

        /// <summary>
        /// Gets the current stock quantity
        /// </summary>
        public int CurrentStock => Inventory?.StockQuantity ?? 0;

        /// <summary>
        /// Gets the minimum stock threshold
        /// </summary>
        public int MinStock => Inventory?.MinimumStock ?? 0;

        /// <summary>
        /// Gets the average rating for the product
        /// </summary>
        public double AverageRating
        {
            get
            {
                if (Reviews == null || !Reviews.Any()) return 0;
                return Reviews.Average(r => r.Rating);
            }
        }

        /// <summary>
        /// Gets the average rating formatted as stars
        /// </summary>
        public string RatingStars
        {
            get
            {
                var rating = AverageRating;
                var fullStars = (int)rating;
                var halfStar = (rating - fullStars) >= 0.5;

                var stars = new string('*', fullStars);
                if (halfStar) stars += "+";

                return $"{rating:F1} ({ReviewCount} reviews)";
            }
        }

        /// <summary>
        /// Gets the review count
        /// </summary>
        public int ReviewCount => Reviews?.Count ?? 0;

        /// <summary>
        /// Gets the category name (null-safe)
        /// </summary>
        public string CategoryName => Category?.CategoryName ?? "Uncategorized";

        /// <summary>
        /// Gets the category icon
        /// </summary>
        public string CategoryIcon => Category?.IconCode ?? "";

        /// <summary>
        /// Gets the active status display
        /// </summary>
        public string ActiveStatus => IsActive ? "Active" : "Inactive";

        /// <summary>
        /// Gets the active status color
        /// </summary>
        public string ActiveStatusColor => IsActive ? "#4CAF50" : "#F44336";

        /// <summary>
        /// Gets the formatted creation date
        /// </summary>
        public string CreatedDateFormatted => CreatedDate.ToString("dd MMM yyyy");

        /// <summary>
        /// Gets a short description (first 100 characters)
        /// </summary>
        public string ShortDescription
        {
            get
            {
                if (string.IsNullOrEmpty(Description)) return "";
                if (Description.Length <= 100) return Description;
                return Description.Substring(0, 100) + "...";
            }
        }

        /// <summary>
        /// Gets tags as a comma-separated string
        /// </summary>
        public string TagsDisplay
        {
            get
            {
                if (Tags == null || !Tags.Any()) return "";
                return string.Join(", ", Tags.Select(t => t.TagName));
            }
        }
    }
}
