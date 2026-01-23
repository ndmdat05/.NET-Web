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
    public class OrderViewModel
    {

        public string Id { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
    }
    public class OrderItem
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;
    }
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime JoinDate { get; set; }
    }


}
