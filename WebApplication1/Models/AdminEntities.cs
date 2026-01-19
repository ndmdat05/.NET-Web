using System;
using System.ComponentModel.DataAnnotations;
namespace WebShop.Models
{
    public class Category
    {
        public String Id { get; set; }
        public String CategoryName { get; set; }
        public String Description { get; set; }

    }
    public class Product
    {
        public String Id { get; set; }
        public String CategoryId { get; set; }
        public String Name { get; set; }
        public String Desc { get; set; }
        public decimal Price { get; set; }
        public decimal salePrice { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime updatedAt { get; set; }

        public String CategoryName { get; set; }
        public String MainImage { get; set; }
    }

}
