using System.Collections.Generic;

namespace WebShop.Models
{
    public class SearchResultViewModel
    {
        public string SearchTerm { get; set; }
        public int TotalCount { get; set; }
        public List<SearchProductItem> Products { get; set; } = new List<SearchProductItem>();
    }

    public class SearchProductItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        public string ImageUrl { get; set; }
        public decimal Rating { get; set; }
        public int ReviewCount { get; set; }
        public string Brand { get; set; }
    }
}
