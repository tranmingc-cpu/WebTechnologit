using System;

namespace WebApplication10.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public decimal? Discount { get; set; }
        public int Quantity { get; set; }
        public int Stock { get; set; }

        public decimal TotalPrice
        {
            get
            {
                var unitPrice = Discount.HasValue && Discount > 0 
                    ? Price - Discount.Value 
                    : Price;
                return unitPrice * Quantity;
            }
        }

        public decimal UnitPrice
        {
            get
            {
                return Discount.HasValue && Discount > 0 
                    ? Price - Discount.Value 
                    : Price;
            }
        }
    }
}
