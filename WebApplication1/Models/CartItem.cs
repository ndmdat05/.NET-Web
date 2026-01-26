namespace WebShop.Models
{
    public class CartItem
    {
        public string ProductId { get; set; }
        public String ProductName { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total => Price * Quantity;

    }
}
