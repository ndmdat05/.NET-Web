using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using WebShop.Models;

namespace WebShop
{
    public class DatabaseService
    {
        private readonly MySqlConnection _connection;

        public DatabaseService(MySqlConnection connection)
        {
            _connection = connection;
        }

        private void OpenConnection()
        {
            if (_connection.State != System.Data.ConnectionState.Open)
                _connection.Open();
        }

        // --- 1. CÁC HÀM HỖ TRỢ HỆ THỐNG (Khôi phục lại cho HomeController) ---

        public bool TestConnection()
        {
            try
            {
                OpenConnection();
                Console.WriteLine("✅ Kết nối đến MySQL thành công!");
                _connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Kết nối đến MySQL thất bại: {ex.Message}");
                return false;
            }
        }

        // Hàm này dùng cho HomeController để tô đỏ trái tim ở trang chủ
        // Đã sửa return List<string> để khớp với ID trong DB
        public List<string> GetWishlistProductIds(string userId)
        {
            var ids = new List<string>();
            try
            {
                OpenConnection();
                // Lưu ý: tên cột trong DB là product_id (theo file SQL của bạn)
                string sql = "SELECT product_id FROM Wishlists WHERE user_id = @userId";
                using var cmd = new MySqlCommand(sql, _connection);
                cmd.Parameters.AddWithValue("@userId", userId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ids.Add(reader["product_id"].ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi GetWishlistProductIds: " + ex.Message);
            }
            finally { _connection.Close(); }
            return ids;
        }

        // --- 2. CÁC HÀM CHO TRANG WISHLIST (Đã cập nhật string ID) ---

        public List<WishlistViewModel> GetWishlist(string userId)
        {
            var list = new List<WishlistViewModel>();
            try
            {
                OpenConnection();
                string sql = @"
                    SELECT p.id, p.name, p.price, img.image_url 
                    FROM Wishlists w
                    JOIN Products p ON w.product_id = p.id
                    LEFT JOIN Product_Images img ON p.id = img.product_id AND img.is_main = 1
                    WHERE w.user_id = @userId";

                using var cmd = new MySqlCommand(sql, _connection);
                cmd.Parameters.AddWithValue("@userId", userId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new WishlistViewModel
                    {
                        ProductId = reader["id"].ToString(),
                        ProductName = reader["name"].ToString(),
                        Price = Convert.ToDecimal(reader["price"]),
                        ImageUrl = reader["image_url"] != DBNull.Value ? reader["image_url"].ToString() : "/images/default.png"
                    });
                }
            }
            catch (Exception ex) { Console.WriteLine("Lỗi GetWishlist: " + ex.Message); }
            finally { _connection.Close(); }
            return list;
        }

        public bool ToggleWishlist(string userId, string productId)
        {
            try
            {
                OpenConnection();
                // Check tồn tại
                string checkSql = "SELECT id FROM Wishlists WHERE user_id = @u AND product_id = @p";
                using var cmdCheck = new MySqlCommand(checkSql, _connection);
                cmdCheck.Parameters.AddWithValue("@u", userId);
                cmdCheck.Parameters.AddWithValue("@p", productId);

                var existingId = cmdCheck.ExecuteScalar();

                if (existingId != null)
                {
                    // Xóa
                    string delSql = "DELETE FROM Wishlists WHERE id = @id";
                    using var cmdDel = new MySqlCommand(delSql, _connection);
                    cmdDel.Parameters.AddWithValue("@id", existingId);
                    cmdDel.ExecuteNonQuery();
                    return false; // Đã bỏ thích
                }
                else
                {
                    // Thêm (Tạo ID mới)
                    string newId = Guid.NewGuid().ToString();
                    string insSql = "INSERT INTO Wishlists (id, user_id, product_id) VALUES (@id, @u, @p)";
                    using var cmdIns = new MySqlCommand(insSql, _connection);
                    cmdIns.Parameters.AddWithValue("@id", newId);
                    cmdIns.Parameters.AddWithValue("@u", userId);
                    cmdIns.Parameters.AddWithValue("@p", productId);
                    cmdIns.ExecuteNonQuery();
                    return true; // Đã thích
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi ToggleWishlist: " + ex.Message);
                return false;
            }
            finally { _connection.Close(); }
        }

        // --- 3. CÁC HÀM CHO TRANG ORDER HISTORY ---

        public List<HistoryOrderViewModel> GetUserOrders(string userId)
        {
            var list = new List<HistoryOrderViewModel>();
            try
            {
                OpenConnection();
                string sql = "SELECT id, order_date, total_amount, order_status FROM Orders WHERE user_id = @userId ORDER BY order_date DESC";
                using var cmd = new MySqlCommand(sql, _connection);
                cmd.Parameters.AddWithValue("@userId", userId);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new HistoryOrderViewModel
                    {
                        OrderId = reader["id"].ToString(),
                        OrderDate = Convert.ToDateTime(reader["order_date"]),
                        TotalAmount = Convert.ToDecimal(reader["total_amount"]),
                        Status = reader["order_status"].ToString()
                    });
                }
            }
            catch (Exception ex) { Console.WriteLine("Lỗi GetUserOrders: " + ex.Message); }
            finally { _connection.Close(); }
            return list;
        }

        public List<OrderDetailViewModel> GetOrderDetails(string orderId)
        {
            var list = new List<OrderDetailViewModel>();
            try
            {
                OpenConnection();
                string sql = @"
                    SELECT p.id, p.name, img.image_url, oi.quantity, oi.unit_price
                    FROM Order_items oi
                    JOIN Products p ON oi.product_id = p.id
                    LEFT JOIN Product_Images img ON p.id = img.product_id AND img.is_main = 1
                    WHERE oi.order_id = @orderId";

                using var cmd = new MySqlCommand(sql, _connection);
                cmd.Parameters.AddWithValue("@orderId", orderId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new OrderDetailViewModel
                    {
                        ProductId = reader["id"].ToString(),
                        ProductName = reader["name"].ToString(),
                        ImageUrl = reader["image_url"] != DBNull.Value ? reader["image_url"].ToString() : "/images/default.png",
                        Quantity = Convert.ToInt32(reader["quantity"]),
                        UnitPrice = Convert.ToDecimal(reader["unit_price"])
                    });
                }
            }
            catch (Exception ex) { Console.WriteLine("Lỗi GetOrderDetails: " + ex.Message); }
            finally { _connection.Close(); }
            return list;
        }
        // Thêm vào DatabaseService.cs
        public WishlistViewModel GetProductById(string id)
        {
            WishlistViewModel product = null;
            try
            {
                OpenConnection();
                // Lấy thông tin sản phẩm và ảnh đại diện (is_main = 1)
                string sql = @"
            SELECT p.id, p.name, p.price, p.sale_price, img.image_url 
            FROM Products p
            LEFT JOIN Product_Images img ON p.id = img.product_id AND img.is_main = 1
            WHERE p.id = @id";

                using var cmd = new MySqlCommand(sql, _connection);
                cmd.Parameters.AddWithValue("@id", id);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    decimal price = Convert.ToDecimal(reader["price"]);
                    decimal salePrice = reader["sale_price"] != DBNull.Value ? Convert.ToDecimal(reader["sale_price"]) : 0;

                    product = new WishlistViewModel
                    {
                        ProductId = reader["id"].ToString(),
                        ProductName = reader["name"].ToString(),
                        // Nếu có giá giảm thì lấy giá giảm, không thì lấy giá gốc
                        Price = salePrice > 0 ? salePrice : price,
                        ImageUrl = reader["image_url"] != DBNull.Value ? reader["image_url"].ToString() : "/images/default.png"
                    };
                }
            }
            catch (Exception ex) { Console.WriteLine("Lỗi GetProductById: " + ex.Message); }
            finally { _connection.Close(); }
            return product;
        }
    }
}