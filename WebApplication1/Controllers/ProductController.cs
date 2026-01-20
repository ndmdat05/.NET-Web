using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using WebShop.Models;
using System.Collections.Generic;
using System;


namespace WebShop.Controllers
{
    public class ProductController : Controller
    {

        private readonly IConfiguration _configuration;
        public ProductController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private string GetConnectionString() => _configuration.GetConnectionString("DefaultConnection");

        // Giả sử Action bạn đặt tên là Detail hoặc ProductDetail
        public IActionResult Detail(String id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction("Index", "Home");

            var model = new ProductDetailViewModel();
            model.Id = id;

            using (var conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                // Lấy thông tin sản phẩm + đánh giá
                string sqlProduct = @"
                    SELECT p.*, 
                           (SELECT AVG(rating) FROM Reviews WHERE product_id = p.id) as avg_rating,
                           (SELECT COUNT(*) FROM Reviews WHERE product_id = p.id) as total_reviews
                    FROM Products p 
                    WHERE p.id = @id";

                using (var cmd = new MySqlCommand(sqlProduct, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model.Name = reader["name"].ToString();
                            model.Price = Convert.ToDecimal(reader["price"]);
                            model.SalePrice = reader["sale_price"] != DBNull.Value ? Convert.ToDecimal(reader["sale_price"]) : null;
                            model.Description = reader["desc"].ToString();
                            model.StockQuantity = Convert.ToInt32(reader["quantity"]);
                            model.CategoryId = reader["category_id"].ToString();

                            //Rating
                            model.AverageRating = reader["avg_rating"] != DBNull.Value ? Convert.ToDecimal(reader["avg_rating"]) : 0;
                            model.ReviewCount = Convert.ToInt32(reader["total_reviews"]);
                        }
                        else
                        {
                            return NotFound(); // Không tìm thấy sp
                        }
                    }
                }

                //Lấy hình ảnh
                string sqlImages = "SELECT image_url FROM Product_Images WHERE product_id = @id ORDER BY is_main DESC";
                using (var cmd = new MySqlCommand(sqlImages, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            model.Images.Add(reader["image_url"].ToString());
                        }
                    }
                }

                // Lấy danh sách Review
                string sqlReviews = @"
                    SELECT r.rating, r.comment, u.email 
                    FROM Reviews r
                    JOIN Users u ON r.user_id = u.id
                    WHERE r.product_id = @id
                    ORDER BY r.id DESC LIMIT 5";

                using (var cmd = new MySqlCommand(sqlReviews, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            model.Reviews.Add(new ReviewViewModel
                            {
                                UserName = reader["email"].ToString(),
                                Rating = Convert.ToDecimal(reader["rating"]),
                                Comment = reader["comment"].ToString()
                            });
                        }
                    }
                }

                // Hàng cùng loại (Category, trừ sản phẩm hiện tại)
                if (!string.IsNullOrEmpty(model.CategoryId))
                {
                    string sqlRelated = @"
                        SELECT p.id, p.name, p.price, pi.image_url 
                        FROM Products p
                        LEFT JOIN Product_Images pi ON p.id = pi.product_id AND pi.is_main = 1
                        WHERE p.category_id = @catId AND p.id != @currentId
                        LIMIT 4";

                    using (var cmd = new MySqlCommand(sqlRelated, conn))
                    {
                        cmd.Parameters.AddWithValue("@catId", model.CategoryId);
                        cmd.Parameters.AddWithValue("@currentId", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                model.RelatedProducts.Add(new Product
                                {
                                    Id = reader["id"].ToString(),
                                    Name = reader["name"].ToString(),
                                    Price = Convert.ToDecimal(reader["price"]),
                                    MainImage = reader["image_url"] != DBNull.Value ? reader["image_url"].ToString() : "/images/default.png"
                                });
                            }
                        }
                    }
                }
            }
            return View("ProductDetail", model);
        }
       
    


        public IActionResult SearchResult(string q)
        {
            return View("SearchResult"); // Hoặc return View() cũng được vì tên khớp
        }
    }
}
