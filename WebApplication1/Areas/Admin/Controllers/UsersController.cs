using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using WebShop.Models; // Đã đồng bộ Namespace

namespace WebShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly MySqlConnection _conn;

        public UsersController(MySqlConnection conn)
        {
            _conn = conn;
        }

        public IActionResult Index()
        {
            List<UserViewModelAdmin> list = new List<UserViewModelAdmin>();

            // Mở kết nối an toàn
            if (_conn.State == ConnectionState.Closed) _conn.Open();

            try
            {
                // Truy vấn lấy tất cả user (LEFT JOIN để không mất user nếu thiếu info)
                string sql = @"
                            SELECT u.id, u.email, u.role, u.`lock`, 
                                   ui.name, ui.phone_num, ui.location
                            FROM Users u
                            LEFT JOIN user_infos ui ON u.id = ui.id";

                using (var cmd = new MySqlCommand(sql, _conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new UserViewModelAdmin
                        {
                            Id = reader["id"].ToString(),

                            Email = reader["email"].ToString(),
                            Role = reader["role"] != DBNull.Value ? reader["role"].ToString() : "customer",
                            FullName = reader["name"] != DBNull.Value ? reader["name"].ToString() : "Chưa cập nhật",
                            Phone = reader["phone_num"] != DBNull.Value ? reader["phone_num"].ToString() : "---",
                            Address = reader["location"] != DBNull.Value ? reader["location"].ToString() : "---",
                            IsLocked = reader["lock"] != DBNull.Value && Convert.ToBoolean(reader["lock"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Ghi lỗi ra TempData để bạn thấy trên web nếu có sự cố SQL
                TempData["Error"] = "Lỗi: " + ex.Message;
            }
            finally { _conn.Close(); }

            return View(list);
        }
    }
}