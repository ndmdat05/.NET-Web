using System.Collections.Generic;
namespace WebShop.Models
{
    public class CollectionViewModel
    {
        public string CurrentCategoryName { get; set; } = "Tất cả sản phẩm";
        public string CurrentCategoryId { get; set; }
        public string CurrentSort { get; set; }
        public string CurrentKeyword { get; set; }
        public List<ProductCollectionItem> Products { get; set; } = new List<ProductCollectionItem>();
    }
    public class ProductCollectionItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        public string ImageUrl { get; set; }
        public decimal Rating { get; set; }
        public int ReviewCount { get; set; }
    }
}
