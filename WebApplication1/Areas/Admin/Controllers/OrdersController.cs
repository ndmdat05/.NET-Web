using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using WebShop.Models;

namespace WebShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly MySqlConnection _conn;

        public OrdersController(MySqlConnection conn)
        {
            _conn = conn;
        }

        // --- 1. TRANG DANH SÁCH (Index) ---
        public IActionResult Index()
        {
            List<OrderViewModel> list = new List<OrderViewModel>();
            if (_conn.State == ConnectionState.Closed) _conn.Open();

            try
            {
                // Lấy danh sách đơn hàng giảm dần theo ngày
                string sql = @"
                    SELECT o.id, u.email, ui.name, o.total_amount, o.order_status, o.order_date 
                    FROM Orders o
                    LEFT JOIN Users u ON o.user_id = u.id
                    LEFT JOIN user_infos ui ON u.id = ui.id
                    ORDER BY o.order_date DESC";

                using (var cmd = new MySqlCommand(sql, _conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new OrderViewModel
                        {
                            Id = reader["id"].ToString(),
                            // Nếu có tên thì lấy tên, không thì lấy email
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
                // Ghi lỗi tạm thời để debug
                Console.WriteLine("Lỗi SQL Index: " + ex.Message);
            }
            finally { _conn.Close(); }

            return View(list);
        }

        // --- 2. TRANG CHI TIẾT (Detail) ---
        public IActionResult Detail(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            if (_conn.State == ConnectionState.Closed) _conn.Open();

            OrderViewModel order = null;
            List<OrderItem> items = new List<OrderItem>();

            try
            {
                // A. Lấy thông tin đơn hàng
                string sqlOrder = @"
                    SELECT o.id, u.email, o.total_amount, o.order_status, o.order_date,
                           ui.name as receiver_name, ui.phone_num, ui.location
                    FROM Orders o
                    LEFT JOIN Users u ON o.user_id = u.id
                    LEFT JOIN user_infos ui ON u.id = ui.id
                    WHERE o.id = @id";

                using (var cmd = new MySqlCommand(sqlOrder, _conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            order = new OrderViewModel
                            {
                                Id = reader["id"].ToString(),
                                CustomerName = reader["receiver_name"] != DBNull.Value ? reader["receiver_name"].ToString() : reader["email"].ToString(),
                                Phone = reader["phone_num"] != DBNull.Value ? reader["phone_num"].ToString() : "Chưa cập nhật",
                                Address = reader["location"] != DBNull.Value ? reader["location"].ToString() : "Chưa cập nhật",
                                TotalAmount = Convert.ToDecimal(reader["total_amount"]),
                                Status = reader["order_status"].ToString(),
                                OrderDate = Convert.ToDateTime(reader["order_date"])
                            };
                        }
                    }
                }

                if (order == null) return NotFound();

                // B. Lấy danh sách sản phẩm
                string sqlItems = @"
                    SELECT oi.quantity, oi.unit_price, p.name, pi.image_url
                    FROM Order_items oi
                    JOIN Products p ON oi.product_id = p.id
                    LEFT JOIN Product_Images pi ON p.id = pi.product_id AND pi.is_main = 1
                    WHERE oi.order_id = @id";

                using (var cmd = new MySqlCommand(sqlItems, _conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new OrderItem
                            {
                                ProductName = reader["name"].ToString(),
                                Quantity = Convert.ToInt32(reader["quantity"]),
                                UnitPrice = Convert.ToDecimal(reader["unit_price"]),
                                ProductImage = reader["image_url"] != DBNull.Value ? reader["image_url"].ToString() : "/images/default.png"
                            });
                        }
                    }
                }

                ViewBag.Items = items;
            }
            finally { _conn.Close(); }

            return View(order);
        }

        // --- 3. CẬP NHẬT TRẠNG THÁI (UpdateStatus) ---
        [HttpPost]
        public IActionResult UpdateStatus(string id, string status)
        {
            if (_conn.State == ConnectionState.Closed) _conn.Open();
            try
            {
                string sql = "UPDATE Orders SET order_status = @status WHERE id = @id";
                using (var cmd = new MySqlCommand(sql, _conn))
                {
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
                TempData["Success"] = "Cập nhật trạng thái thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
            }
            finally { _conn.Close(); }

            return RedirectToAction("Detail", new { id = id });
        }
    }
}