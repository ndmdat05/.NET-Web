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

        public IActionResult Detail(String id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction("Index", "Home");

            var model = new ProductDetailViewModel();
            model.Id = id;

            using (var conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

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

                            model.AverageRating = reader["avg_rating"] != DBNull.Value ? Convert.ToDecimal(reader["avg_rating"]) : 0;
                            model.ReviewCount = Convert.ToInt32(reader["total_reviews"]);
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                }
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

                string sqlVariants = "SELECT * FROM Product_variants WHERE product_id = @id ORDER BY weight ASC";

                using (var cmd = new MySqlCommand(sqlVariants, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (model.Variants == null) model.Variants = new List<ProductVariantViewModel>();
                        while (reader.Read())
                        {
                            decimal adjustment = reader["price_adjustment"] != DBNull.Value ? Convert.ToDecimal(reader["price_adjustment"]) : 0;
                            decimal basePriceForCalc = (model.SalePrice.HasValue && model.SalePrice.Value > 0) ? model.SalePrice.Value : model.Price;

                            model.Variants.Add(new ProductVariantViewModel
                            {
                                Id = reader["id"] != DBNull.Value ? reader["id"].ToString() : "",
                                Weight = reader["weight"] != DBNull.Value ? Convert.ToDecimal(reader["weight"]) : 0,
                                Stock = reader["stock_quantity"] != DBNull.Value ? Convert.ToInt32(reader["stock_quantity"]) : 0,
                                FinalPrice = basePriceForCalc + adjustment,
                                VariantName = reader["variant_name"] != DBNull.Value ? reader["variant_name"].ToString() : null
                            });
                        }
                    }
                }

                if (!string.IsNullOrEmpty(model.CategoryId))
                {
                    string sqlRelated = @"SELECT p.id, p.name, p.price, p.sale_price, pi.image_url 
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
                                    salePrice = reader["sale_price"] != DBNull.Value ? Convert.ToDecimal(reader["sale_price"]) : 0,
                                    MainImage = reader["image_url"] != DBNull.Value ? reader["image_url"].ToString() : "/images/default.png"
                                });
                            }
                        }
                    }
                }
            }
            return View("ProductDetail", model);
        }

        public IActionResult SearchResult(string q, string sort)
        {
            var model = new SearchResultViewModel();
            model.SearchTerm = string.IsNullOrEmpty(q) ? "" : q;
            model.CurrentSort = sort;

            if (string.IsNullOrEmpty(q))
            {
                return View("SearchResult", model);
            }

            using (var conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();
                string sql = @"
                    SELECT p.id, p.name, p.price, p.sale_price, pi.image_url, 
                           (SELECT AVG(rating) FROM Reviews WHERE product_id = p.id) as avg_rating,
                           (SELECT COUNT(*) FROM Reviews WHERE product_id = p.id) as total_reviews
                    FROM Products p
                    LEFT JOIN Product_Images pi ON p.id = pi.product_id AND pi.is_main = 1
                    WHERE p.name LIKE @keyword";
                if (sort == "price-asc")
                {
                    sql += " ORDER BY p.price ASC";
                }
                else
                {
                    sql += " ORDER BY p.created_time DESC";
                }

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@keyword", "%" + q + "%");

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            model.Products.Add(new SearchProductItem
                            {
                                Id = reader["id"].ToString(),
                                Name = reader["name"].ToString(),
                                Price = Convert.ToDecimal(reader["price"]),
                                SalePrice = reader["sale_price"] != DBNull.Value ? Convert.ToDecimal(reader["sale_price"]) : null,
                                ImageUrl = reader["image_url"] != DBNull.Value ? reader["image_url"].ToString() : "/images/default.png",
                                Rating = reader["avg_rating"] != DBNull.Value ? Convert.ToDecimal(reader["avg_rating"]) : 0,
                                ReviewCount = Convert.ToInt32(reader["total_reviews"]),
                                Brand = "Paddy"
                            });
                        }
                    }
                }
            }

            model.TotalCount = model.Products.Count;
            return View("SearchResult", model);
        }
    }
}