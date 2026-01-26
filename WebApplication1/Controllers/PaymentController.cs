using Microsoft.AspNetCore.Mvc;
using WebShop.Models;
using WebShop.Helpers;
using MySql.Data.MySqlClient;

namespace WebShop.Controllers
{
    public class PaymentController : Controller
    {
        private readonly string _connectionString = "Server=127.0.0.1;Database=DOCNET;User Id=root;Password=123456;Port=3306;";

        // Hiển thị trang thanh toán
        public IActionResult Index()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();

            if (cart.Count == 0)
                return RedirectToAction("Index", "Cart");

            return View("~/Views/Cart/Payment.cshtml", cart);
        }

        // Xử lý đặt hàng
        [HttpPost]
        public IActionResult CheckOut(string phone, string address, string firstName, string lastName, string city, string paymentMethod, string shippingMethod)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart");
            if (cart == null || cart.Count == 0)
                return RedirectToAction("Index", "Cart");

            string userId = HttpContext.Session.GetString("UserId");
            decimal subtotal = cart.Sum(i => i.Total);
            decimal shippingFee = string.IsNullOrEmpty(shippingMethod) ? 25000 : decimal.Parse(shippingMethod);
            decimal totalAmount = subtotal + shippingFee;
            string orderId = "ORD" + DateTime.Now.Ticks;

            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // INSERT Orders
                            string sqlOrder = @"INSERT INTO Orders (id, user_id, order_status, subtotal, total_amount, order_date)
                                               VALUES (@id, @userId, @status, @subtotal, @total, @orderDate)";
                            using (var cmd = new MySqlCommand(sqlOrder, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@id", orderId);
                                cmd.Parameters.AddWithValue("@userId", string.IsNullOrEmpty(userId) ? DBNull.Value : userId);
                                cmd.Parameters.AddWithValue("@status", "pending");
                                cmd.Parameters.AddWithValue("@subtotal", subtotal);
                                cmd.Parameters.AddWithValue("@total", totalAmount);
                                cmd.Parameters.AddWithValue("@orderDate", DateTime.Now);
                                cmd.ExecuteNonQuery();
                            }

                            // INSERT Order_items
                            foreach (var item in cart)
                            {
                                string sqlItem = @"INSERT INTO Order_items (id, product_id, order_id, quantity, unit_price)
                                                  VALUES (@id, @productId, @orderId, @qty, @price)";
                                using (var cmd = new MySqlCommand(sqlItem, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@id", "OI" + Guid.NewGuid().ToString("N").Substring(0, 8));
                                    cmd.Parameters.AddWithValue("@productId", item.ProductId);
                                    cmd.Parameters.AddWithValue("@orderId", orderId);
                                    cmd.Parameters.AddWithValue("@qty", item.Quantity);
                                    cmd.Parameters.AddWithValue("@price", item.Price);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            // INSERT Payments
                            string sqlPayment = @"INSERT INTO Payments (id, order_id, pay_method, pay_status, payment_time)
                                                 VALUES (@id, @orderId, @method, @status, @time)";
                            using (var cmd = new MySqlCommand(sqlPayment, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@id", "PAY" + Guid.NewGuid().ToString("N").Substring(0, 8));
                                cmd.Parameters.AddWithValue("@orderId", orderId);
                                cmd.Parameters.AddWithValue("@method", paymentMethod ?? "cod");
                                cmd.Parameters.AddWithValue("@status", "pending");
                                cmd.Parameters.AddWithValue("@time", DateTime.Now);
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            HttpContext.Session.Remove("Cart");
                            return RedirectToAction("NotifyPayment");
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // Trang thông báo thành công
        public IActionResult NotifyPayment()
        {
            return View("~/Views/Cart/NotifyPayment.cshtml");
        }
    }
}