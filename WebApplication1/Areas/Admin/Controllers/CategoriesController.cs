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
    }
}