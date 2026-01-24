using System.Collections.Generic;

namespace WebShop.Models
{
    public class HomeViewModel
    {
        public List<ProductViewModel> DiscountedProducts { get; set; } = new List<ProductViewModel>();
        public List<ProductViewModel> RecommendedProducts { get; set; } = new List<ProductViewModel>();
    }
    public class ProductViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        public string ImageUrl { get; set; }
    }
}
