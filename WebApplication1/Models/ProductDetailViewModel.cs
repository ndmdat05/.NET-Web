using System.Collections.Generic;

namespace WebShop.Models
{
    public class ProductDetailViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        public string Description { get; set; }
        public int StockQuantity { get; set; }
        public string Brand { get; set; }
        public string CategoryId { get; set; }
        public List<string> Images { get; set; } = new List<string>();
        // Đánh giá sp
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public List<ReviewViewModel> Reviews { get; set; } = new List<ReviewViewModel>();
        // Hàng cùng loại
        public List<Product> RelatedProducts { get; set; } = new List<Product>();
        public List<ProductVariantViewModel> Variants { get; set; } = new List<ProductVariantViewModel>();
    }

    public class ProductVariantViewModel
    {
        public string Id { get; set; }
        public decimal Weight { get; set; }
        public decimal FinalPrice { get; set; }
        public int Stock { get; set; }
    }
    public class ReviewViewModel
    {
        public string UserName { get; set; }
        public decimal Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
