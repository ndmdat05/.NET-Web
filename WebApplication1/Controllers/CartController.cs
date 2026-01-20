using Microsoft.AspNetCore.Mvc;
using WebShop.Models;
using WebShop.Helpers;
using MySql.Data.MySqlClient;

namespace WebShop.Controllers
{
    public class CartController : Controller

    {
        //nay de khai bao connect mysql sau nay se dung dependency injection
        private readonly string _connectionString = "Server=127.0.0.1;Database=DOCNET;User Id=root;Password=123456;Port=3306;";
        public IActionResult Index()
           

        {
            var cart = HttpContext.Session.Get<List<Models.CartItem>>("Cart") ?? new List<CartItem>();
            ViewBag.totalAmount = cart.Sum(item => item.Total);
            return View(cart);
        }
        public IActionResult AddToCart(String id, int quantity = 1)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
            var existingItem = cart.FirstOrDefault(x => x.ProductId == id);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var product = GetProductFromDb(id);
                if (product != null)
                {
                    product.Quantity = quantity;
                    cart.Add(product);
                }
            }

            HttpContext.Session.Set("Cart", cart);
            return RedirectToAction("Index");
        }
        //cap nhat so luong
        public IActionResult Update(string id, int quantity)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart");
            if (cart != null)
            {
                var item = cart.FirstOrDefault(x => x.ProductId == id);
                if (item != null && quantity > 0)
                {
                    item.Quantity = quantity;
                    HttpContext.Session.Set("Cart", cart);
                }
            }
            return RedirectToAction("Index");
        }
        public IActionResult Remove(string id)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart");
            if (cart != null)
            {
                var item = cart.FirstOrDefault(x => x.ProductId == id);
                if (item != null)
                {
                    cart.Remove(item);
                    HttpContext.Session.Set("Cart", cart);
                }
            }
            return RedirectToAction("Index");
        }
        

        private CartItem GetProductFromDb(string id)
        {
            CartItem item = null;
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                // Lấy Tên, Giá và Ảnh chính
                string sql = @"
                    SELECT p.id, p.name, 
                           COALESCE(p.sale_price, p.price) as final_price, 
                           img.image_url
                    FROM Products p
                    LEFT JOIN Product_Images img ON p.id = img.product_id AND img.is_main = 1
                    WHERE p.id = @id";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            item = new CartItem
                            {
                                ProductId = reader["id"].ToString(),
                                ProductName = reader["name"].ToString(),
                                Price = Convert.ToDecimal(reader["final_price"]),
                                imageUrl = reader["image_url"] != DBNull.Value ? reader["image_url"].ToString() : "/images/default.png"
                            };
                        }
                    }
                }
            }
            return item;
        }
        

        public IActionResult Payment() => View();
       

        public IActionResult NotifyPayment()
        {
            return View("NotifyPayment");
        }
    }
}