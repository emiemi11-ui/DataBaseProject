using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ECommerceApp.Models
{
    /// <summary>
    /// Represents an item in the shopping cart.
    /// This is a client-side model, not stored in database.
    /// </summary>
    public class CartItem : INotifyPropertyChanged
    {
        private int _quantity;

        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public string ImageURL { get; set; }

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Subtotal));
                }
            }
        }

        /// <summary>
        /// Calculated subtotal for this cart item
        /// </summary>
        public decimal Subtotal => Quantity * UnitPrice;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Creates a CartItem from a Product
        /// </summary>
        public static CartItem FromProduct(Product product, int quantity = 1)
        {
            return new CartItem
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                UnitPrice = product.Price,
                ImageURL = product.ImageURL,
                Quantity = quantity
            };
        }
    }
}
