using AlgebraWebShop2026.Models;

namespace AlgebraWebShop2026.Extensions
{
    public class CartItem
    {
        public Product Product { get; set; }
        public decimal Quantity { get; set; }

        public decimal getTotal()
        {
            return Product.Price * (100m - Product.Discount) / 100m * Quantity;
        }
    }
}
