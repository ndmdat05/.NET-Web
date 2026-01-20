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
        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Category model)
        {
            try
            {
                if (_conn.State == ConnectionState.Closed) _conn.Open();
                // Tự sinh ID ngẫu nhiên
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
        // --- 4. SỬA DANH MỤC (GET) ---
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
                            Description = reader["description"].ToString()
                        };
                    }
                }
            }
            _conn.Close();
            return View(cat); // Tạo View Edit.cshtml tương tự Create
        }

        // --- 5. LƯU SỬA (POST) ---
        [HttpPost]
        public IActionResult Edit(Category model)
        {
            if (_conn.State == ConnectionState.Closed) _conn.Open();
            string sql = "UPDATE P_category SET category_name=@name, description=@desc WHERE id=@id";
            using (var cmd = new MySqlCommand(sql, _conn))
            {
                cmd.Parameters.AddWithValue("@name", model.CategoryName);
                cmd.Parameters.AddWithValue("@desc", model.Description);
                cmd.Parameters.AddWithValue("@id", model.Id);
                cmd.ExecuteNonQuery();
            }
            _conn.Close();
            return RedirectToAction("Index");
        }

        // --- 6. XÓA DANH MỤC (POST AJAX) ---
        [HttpPost]
        public IActionResult Delete(string id)
        {
            // Cần check xem có sản phẩm nào đang dùng danh mục này không?
            // Nếu có thì chặn xóa để tránh lỗi dữ liệu
            // ... (Code check Count(*) from Products where category_id = id)

            // Nếu an toàn thì xóa: DELETE FROM P_category WHERE id = @id
            // ...
            return Json(new { success = true });
        }
    }
}