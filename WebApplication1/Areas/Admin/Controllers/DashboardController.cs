using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using WebShop.Models;

namespace WebShop.Areas.Admin.Controllers
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
            int totalUsers = 0;
            int totalOrders = 0;
            decimal totalRevenue = 0;
            int totalProducts = 0;
            List<OrderViewModel> recentOrders = new List<OrderViewModel>();

            if (_conn.State == ConnectionState.Closed) _conn.Open();

            try
            {
                // 1. Đếm User
                using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM Users", _conn))
                    totalUsers = Convert.ToInt32(cmd.ExecuteScalar());

                // 2. Đếm Sản phẩm
                using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM Products", _conn))
                    totalProducts = Convert.ToInt32(cmd.ExecuteScalar());

                // 3. Đếm Đơn hàng
                using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM Orders", _conn))
                    totalOrders = Convert.ToInt32(cmd.ExecuteScalar());

                // 4. Tính Doanh thu
                using (var cmd = new MySqlCommand("SELECT SUM(total_amount) FROM Orders WHERE order_status != 'Cancelled'", _conn))
                {
                    var res = cmd.ExecuteScalar();
                    totalRevenue = res != DBNull.Value ? Convert.ToDecimal(res) : 0;
                }

                // 5. Lấy đơn mới nhất
                string sqlRecent = @"SELECT o.id, u.email, o.total_amount, o.order_status, o.order_date 
                                     FROM Orders o LEFT JOIN Users u ON o.user_id = u.id 
                                     ORDER BY o.order_date DESC LIMIT 5";
                using (var cmd = new MySqlCommand(sqlRecent, _conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        recentOrders.Add(new OrderViewModel
                        {
                            Id = reader["id"].ToString(),
                            CustomerName = reader["email"].ToString(),
                            TotalAmount = Convert.ToDecimal(reader["total_amount"]),
                            Status = reader["order_status"].ToString(),
                            OrderDate = Convert.ToDateTime(reader["order_date"])
                        });
                    }
                }
            }
            catch { } // Bỏ qua lỗi dashboard để không chặn trang chính
            finally { _conn.Close(); }

            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalProducts = totalProducts;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalRevenue = totalRevenue;

            return View(recentOrders);
        }
    }
}