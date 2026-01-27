using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using WebShop.Models;
using System.Data;
namespace WebShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly MySqlConnection _conn;

        public CategoriesController(MySqlConnection conn)
        {
            _conn = conn;
        }
        public IActionResult Index()
        {
            List<Category> list = new List<Category>();
            if (_conn.State == ConnectionState.Closed) _conn.Open();

            using (var cmd = new MySqlCommand("SELECT * FROM P_category", _conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new Category
                    {
                        Id = reader["id"].ToString(),
                        CategoryName = reader["category_name"].ToString(),
                        Description = reader["description"].ToString()
                    });
                }
            }
            _conn.Close();
            return View(list);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category model)
        {
            try
            {
                if (_conn.State == ConnectionState.Closed) _conn.Open();
                string newId = Guid.NewGuid().ToString();
                string sql = "INSERT INTO P_category (id, category_name, description) VALUES (@id, @name, @desc)";
                using (var cmd = new MySqlCommand(sql, _conn))
                {
                    cmd.Parameters.AddWithValue("@id", newId);
                    cmd.Parameters.AddWithValue("@name", model.CategoryName);
                    cmd.Parameters.AddWithValue("@desc", model.Description);
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(model);
            }
            finally { _conn.Close(); }
        }
        public IActionResult Edit(string id)
        {
            if (_conn.State == ConnectionState.Closed) _conn.Open();
            Category cat = null;

            using (var cmd = new MySqlCommand("SELECT * FROM P_category WHERE id=@id", _conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        cat = new Category
                        {
                            Id = reader["id"].ToString(),
                            CategoryName = reader["category_name"].ToString(),
                            Description = reader["description"] != DBNull.Value ? reader["description"].ToString() : ""
                        };
                    }
                }
            }
            _conn.Close();

            if (cat == null) return NotFound();
            return View(cat);
        }

        [HttpPost]
        public IActionResult Edit(Category model)
        {
            if (_conn.State == ConnectionState.Closed) _conn.Open();
            try
            {
                string sql = "UPDATE P_category SET category_name=@name, description=@desc WHERE id=@id";
                using (var cmd = new MySqlCommand(sql, _conn))
                {
                    cmd.Parameters.AddWithValue("@name", model.CategoryName);
                    cmd.Parameters.AddWithValue("@desc", model.Description);
                    cmd.Parameters.AddWithValue("@id", model.Id);
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(model);
            }
            finally { _conn.Close(); }
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult Delete(string id)
        {
            if (_conn.State == System.Data.ConnectionState.Closed) _conn.Open();
            try
            {

                string sqlCheck = "SELECT COUNT(*) FROM Products WHERE category_id = @id";
                using (var cmdCheck = new MySqlCommand(sqlCheck, _conn))
                {
                    cmdCheck.Parameters.AddWithValue("@id", id);
                    long count = Convert.ToInt64(cmdCheck.ExecuteScalar());

                    if (count > 0)
                    {
                        return Json(new { success = false, message = $"Không thể xóa! Có {count} sản phẩm đang thuộc danh mục này." });
                    }
                }

                string sqlDel = "DELETE FROM P_category WHERE id = @id";
                using (var cmdDel = new MySqlCommand(sqlDel, _conn))
                {
                    cmdDel.Parameters.AddWithValue("@id", id);
                    int rowsAffected = cmdDel.ExecuteNonQuery();

                    if (rowsAffected > 0)
                        return Json(new { success = true, message = "Đã xóa danh mục thành công!" });
                    else
                        return Json(new { success = false, message = "Không tìm thấy danh mục để xóa." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi Server: " + ex.Message });
            }
            finally
            {
                _conn.Close();
            }
        }
    }
}