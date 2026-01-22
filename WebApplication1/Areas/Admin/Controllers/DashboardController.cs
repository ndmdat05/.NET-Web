using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using WebShop.Models; // Đảm bảo đúng namespace của bạn

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly MySqlConnection _conn;

        public DashboardController(MySqlConnection conn)
        {
            _conn = conn;
        }

        public IActionResult Index()
        {
            // 1. Biến để chứa số liệu thống kê
            int totalOrders = 0;
            decimal totalRevenue = 0;
            int totalUsers = 0;
            int totalProducts = 0;
            List<OrderViewModel> recentOrders = new List<OrderViewModel>();

            try
            {
                if (_conn.State == ConnectionState.Closed) _conn.Open();

                // 2. Truy vấn thống kê số lượng
                using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM Orders", _conn))
                    totalOrders = Convert.ToInt32(cmd.ExecuteScalar());

                using (var cmd = new MySqlCommand("SELECT SUM(total_amount) FROM Orders WHERE order_status != 'Cancelled'", _conn))
                {
                    var result = cmd.ExecuteScalar();
                    totalRevenue = result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                }

                using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM Users", _conn))
                    totalUsers = Convert.ToInt32(cmd.ExecuteScalar());

                using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM Products", _conn))
                    totalProducts = Convert.ToInt32(cmd.ExecuteScalar());

                // 3. Lấy 5 đơn hàng mới nhất
                string sqlRecent = @"
                    SELECT o.id, u.email, o.total_amount, o.order_status, o.order_date, ui.name
                    FROM Orders o
                    LEFT JOIN Users u ON o.user_id = u.id
                    LEFT JOIN user_infos ui ON u.id = ui.id
                    ORDER BY o.order_date DESC LIMIT 5";

                using (var cmd = new MySqlCommand(sqlRecent, _conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        recentOrders.Add(new OrderViewModel
                        {
                            Id = reader["id"].ToString(),
                            CustomerName = reader["name"] != DBNull.Value ? reader["name"].ToString() : reader["email"].ToString(),
                            TotalAmount = Convert.ToDecimal(reader["total_amount"]),
                            Status = reader["order_status"].ToString(),
                            OrderDate = Convert.ToDateTime(reader["order_date"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi kết nối: " + ex.Message;
            }
            finally { _conn.Close(); }

            // 4. Gửi dữ liệu sang View
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalProducts = totalProducts;

            return View(recentOrders);
        }
    }
}