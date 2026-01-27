using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using WebShop.Models;
using System.Data;

namespace WebShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly MySqlConnection _conn;
        private readonly IWebHostEnvironment _env;

        public ProductsController(MySqlConnection conn, IWebHostEnvironment env)
        {
            _conn = conn;
            _env = env;
        }

        public IActionResult Index()
        {
            List<Product> list = new List<Product>();
            if (_conn.State == ConnectionState.Closed) _conn.Open();

            string sql = @"
                SELECT p.*, c.category_name, img.image_url 
                FROM Products p
                LEFT JOIN P_category c ON p.category_id = c.id
                LEFT JOIN Product_Images img ON p.id = img.product_id AND img.is_main = 1
                ORDER BY p.created_time DESC";

            using (var cmd = new MySqlCommand(sql, _conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new Product
                    {
                        Id = reader["id"].ToString(),
                        Name = reader["name"].ToString(),
                        Price = reader["price"] != DBNull.Value ? Convert.ToDecimal(reader["price"]) : 0,
                        Quantity = reader["quantity"] != DBNull.Value ? Convert.ToInt32(reader["quantity"]) : 0,
                        CategoryName = reader["category_name"] != DBNull.Value ? reader["category_name"].ToString() : "Chưa phân loại",
                        MainImage = reader["image_url"] != DBNull.Value ? reader["image_url"].ToString() : "/images/default.png"
                    });
                }
            }
            _conn.Close();
            return View(list);
        }


        public IActionResult Create()
        {

            List<Category> categories = new List<Category>();

            if (_conn.State == ConnectionState.Closed) _conn.Open();
            using (var cmd = new MySqlCommand("SELECT id, category_name FROM P_category", _conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    categories.Add(new Category
                    {
                        Id = reader["id"].ToString(),
                        CategoryName = reader["category_name"].ToString()
                    });
                }
            }
            _conn.Close();

            ViewBag.Categories = categories; 
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(Product model, IFormFile ImageFile)
        {
            if (_conn.State == ConnectionState.Closed) _conn.Open();


            using (var transaction = _conn.BeginTransaction())
            {
                try
                {

                    string newProductId = Guid.NewGuid().ToString(); // Tạo ID mới

                    string sqlProduct = @"
                        INSERT INTO Products (id, name, price, quantity, category_id, `desc`, created_time) 
                        VALUES (@id, @name, @price, @qty, @catId, @desc, NOW())";

                    using (var cmd = new MySqlCommand(sqlProduct, _conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@id", newProductId);
                        cmd.Parameters.AddWithValue("@name", model.Name);
                        cmd.Parameters.AddWithValue("@price", model.Price);
                        cmd.Parameters.AddWithValue("@qty", model.Quantity);
                        cmd.Parameters.AddWithValue("@catId", model.CategoryId);
                        cmd.Parameters.AddWithValue("@desc", model.Desc);
                        cmd.ExecuteNonQuery();
                    }

                    if (ImageFile != null && ImageFile.Length > 0)
                    {

                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                        string folderPath = Path.Combine(_env.WebRootPath, "images", "products");

                        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                        string filePath = Path.Combine(folderPath, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);
                        }

                        string dbPath = "/images/products/" + fileName;
                        string newImgId = Guid.NewGuid().ToString();

                        string sqlImg = @"INSERT INTO Product_Images (id, product_id, image_url, is_main) 
                                          VALUES (@id, @pid, @url, 1)";

                        using (var cmdImg = new MySqlCommand(sqlImg, _conn, transaction))
                        {
                            cmdImg.Parameters.AddWithValue("@id", newImgId);
                            cmdImg.Parameters.AddWithValue("@pid", newProductId);
                            cmdImg.Parameters.AddWithValue("@url", dbPath);
                            cmdImg.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit(); // Chốt lưu dữ liệu
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // Nếu lỗi thì hủy hết
                    TempData["Error"] = "Lỗi: " + ex.Message;
                    return RedirectToAction("Create");
                }
                finally { _conn.Close(); }
            }
        }

        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            Product product = null;
            List<Category> categories = new List<Category>();

            if (_conn.State == ConnectionState.Closed) _conn.Open();

            string sql = @"
        SELECT p.*, img.image_url 
        FROM Products p
        LEFT JOIN Product_Images img ON p.id = img.product_id AND img.is_main = 1
        WHERE p.id = @id";

            using (var cmd = new MySqlCommand(sql, _conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        product = new Product
                        {
                            Id = reader["id"].ToString(),
                            Name = reader["name"].ToString(),
                            Price = reader["price"] != DBNull.Value ? Convert.ToDecimal(reader["price"]) : 0,
                            Quantity = reader["quantity"] != DBNull.Value ? Convert.ToInt32(reader["quantity"]) : 0,
                            CategoryId = reader["category_id"] != DBNull.Value ? reader["category_id"].ToString() : "",
                            Desc = reader["desc"] != DBNull.Value ? reader["desc"].ToString() : "",
                            MainImage = reader["image_url"] != DBNull.Value ? reader["image_url"].ToString() : "/images/default.png"
                        };
                    }
                }
            }

            if (product == null) return NotFound();

            using (var cmd = new MySqlCommand("SELECT id, category_name FROM P_category", _conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    categories.Add(new Category
                    {
                        Id = reader["id"].ToString(),
                        CategoryName = reader["category_name"].ToString()
                    });
                }
            }
            _conn.Close();

            ViewBag.Categories = categories;
            return View(product); // Trả về View Edit cùng dữ liệu sản phẩm
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, Product model, IFormFile? ImageFile)
        {
            if (_conn.State == ConnectionState.Closed) _conn.Open();

            using (var transaction = _conn.BeginTransaction())
            {
                try
                {

                    string sqlUpdate = @"
                UPDATE Products 
                SET name = @name, price = @price, quantity = @qty, category_id = @cat, `desc` = @desc
                WHERE id = @id";

                    using (var cmd = new MySqlCommand(sqlUpdate, _conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@name", model.Name);
                        cmd.Parameters.AddWithValue("@price", model.Price);
                        cmd.Parameters.AddWithValue("@qty", model.Quantity);
                        cmd.Parameters.AddWithValue("@cat", model.CategoryId);
                        cmd.Parameters.AddWithValue("@desc", model.Desc);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }


                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        // 1. Upload ảnh mới
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                        string savePath = Path.Combine(_env.WebRootPath, "images", "products", fileName);
                        using (var stream = new FileStream(savePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);
                        }
                        string newDbPath = "/images/products/" + fileName;

                        string checkImgSql = "SELECT id FROM Product_Images WHERE product_id = @pid AND is_main = 1";
                        string imgId = null;

                        using (var cmdCheck = new MySqlCommand(checkImgSql, _conn, transaction))
                        {
                            cmdCheck.Parameters.AddWithValue("@pid", id);
                            imgId = cmdCheck.ExecuteScalar()?.ToString();
                        }

                        if (imgId != null)
                        {
                            // Đã có ảnh -> Update
                            string updateImg = "UPDATE Product_Images SET image_url = @url WHERE id = @imgId";
                            using (var cmdUp = new MySqlCommand(updateImg, _conn, transaction))
                            {
                                cmdUp.Parameters.AddWithValue("@url", newDbPath);
                                cmdUp.Parameters.AddWithValue("@imgId", imgId);
                                cmdUp.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Chưa có ảnh -> Insert mới
                            string insertImg = "INSERT INTO Product_Images (id, product_id, image_url, is_main) VALUES (@nid, @pid, @url, 1)";
                            using (var cmdIn = new MySqlCommand(insertImg, _conn, transaction))
                            {
                                cmdIn.Parameters.AddWithValue("@nid", Guid.NewGuid().ToString());
                                cmdIn.Parameters.AddWithValue("@pid", id);
                                cmdIn.Parameters.AddWithValue("@url", newDbPath);
                                cmdIn.ExecuteNonQuery();
                            }
                        }
                    }

                    transaction.Commit();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    TempData["Error"] = "Lỗi: " + ex.Message;
                    return RedirectToAction("Index");
                }
                finally { _conn.Close(); }
            }
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            if (_conn.State == ConnectionState.Closed) _conn.Open();
            using (var transaction = _conn.BeginTransaction())
            {
                try
                {

                    string getImgSql = "SELECT image_url FROM Product_Images WHERE product_id = @id";
                    List<string> imagesToDelete = new List<string>();

                    using (var cmd = new MySqlCommand(getImgSql, _conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader["image_url"] != DBNull.Value)
                                    imagesToDelete.Add(reader["image_url"].ToString());
                            }
                        }
                    }


                    string delImgSql = "DELETE FROM Product_Images WHERE product_id = @id";
                    using (var cmd = new MySqlCommand(delImgSql, _conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }

                    string delProdSql = "DELETE FROM Products WHERE id = @id";
                    using (var cmd = new MySqlCommand(delProdSql, _conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    foreach (var imgPath in imagesToDelete)
                    {
                        string fullPath = Path.Combine(_env.WebRootPath, imgPath.TrimStart('/').Replace('/', '\\'));
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }

                    return Json(new { success = true, message = "Đã xóa sản phẩm thành công" });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, message = "Lỗi: " + ex.Message });
                }
                finally { _conn.Close(); }
            }
        }
    }
}