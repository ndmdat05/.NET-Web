using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using WebShop.Models;
using System.Collections.Generic;
using System;

namespace WebShop.Controllers
{
    public class CollectionsController : Controller
    {
        private readonly IConfiguration _configuration;

        public CollectionsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string GetConnectionString() => _configuration.GetConnectionString("DefaultConnection");
        public IActionResult Index(string categoryId, string sort, string keyword)
        {
            var model = new CollectionViewModel();
            model.CurrentCategoryId = categoryId;
            model.CurrentSort = sort;
            model.CurrentKeyword = keyword;

            if (!string.IsNullOrEmpty(keyword))
                model.CurrentCategoryName = "Danh mục: " + keyword;
            else
                model.CurrentCategoryName = "Tất cả sản phẩm";

            using (var conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string sql = @"
                    SELECT p.id, p.name, p.price, p.sale_price, pi.image_url, c.category_name,
                           (SELECT AVG(rating) FROM Reviews WHERE product_id = p.id) as avg_rating,
                           (SELECT COUNT(*) FROM Reviews WHERE product_id = p.id) as total_reviews
                    FROM Products p
                    LEFT JOIN Product_Images pi ON p.id = pi.product_id AND pi.is_main = 1
                    LEFT JOIN P_category c ON p.category_id = c.id
                    WHERE 1=1";

                if (!string.IsNullOrEmpty(categoryId))
                {
                    sql += " AND p.category_id = @catId";
                }

                if (!string.IsNullOrEmpty(keyword))
                {
                    sql += " AND p.name LIKE @keyword";
                }

                switch (sort)
                {
                    case "alpha-asc": sql += " ORDER BY p.name ASC"; break;
                    case "alpha-desc": sql += " ORDER BY p.name DESC"; break;
                    case "price-asc": sql += " ORDER BY p.price ASC"; break;
                    case "price-desc": sql += " ORDER BY p.price DESC"; break;
                    default: sql += " ORDER BY p.created_time DESC"; break;
                }

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    if (!string.IsNullOrEmpty(categoryId)) cmd.Parameters.AddWithValue("@catId", categoryId);
                    if (!string.IsNullOrEmpty(keyword)) cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (string.IsNullOrEmpty(keyword) && !string.IsNullOrEmpty(categoryId))
                            {
                                model.CurrentCategoryName = reader["category_name"] != DBNull.Value
                                                            ? reader["category_name"].ToString()
                                                            : "Danh mục";
                            }

                            model.Products.Add(new ProductCollectionItem
                            {
                                Id = reader["id"].ToString(),
                                Name = reader["name"].ToString(),
                                Price = Convert.ToDecimal(reader["price"]),
                                SalePrice = reader["sale_price"] != DBNull.Value ? Convert.ToDecimal(reader["sale_price"]) : null,
                                ImageUrl = reader["image_url"] != DBNull.Value ? reader["image_url"].ToString() : "/images/default.png",
                                Rating = reader["avg_rating"] != DBNull.Value ? Convert.ToDecimal(reader["avg_rating"]) : 0,
                                ReviewCount = Convert.ToInt32(reader["total_reviews"])
                            });
                        }
                    }
                }
            }

            return View("collections", model);
        }
    }
}