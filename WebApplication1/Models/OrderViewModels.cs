using System;

namespace WebShop.Models
{
    public class OrderViewModel1
    {
        public string OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } // pending, shipping, completed...
        public decimal TotalAmount { get; set; }
    }

    public class OrderItemViewModel
    {
        public string ProductName { get; set; }
        public string Image { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Quantity * Price;
    }
}