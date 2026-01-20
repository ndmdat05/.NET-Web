using Microsoft.AspNetCore.Mvc;
using WebShop.Models;
using System.Collections.Generic;
using System.Linq;
using WebShop.Helpers;

namespace WebShop.Controllers
{
    public class PaymentController : Controller
    {
        // 1. Hiển thị trang thanh toán
        public IActionResult Index()
                { var cart = HttpContext.Session.Get<List<Models.CartItem>>("Cart") ?? new List<CartItem>();
            if(cart.Count == 0)
            
                return RedirectToAction("Index", "Cart");
            
            return View();
        }
        [HttpPost]
        // 2. Xử lý đặt hàng khi nhấn "Hoàn tất đơn hàng"
        public IActionResult CheckOut(String phone, String address, string paymentMethod)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart");
            if (cart == null || cart.Count == 0) return RedirectToAction("Index", "Home");
            // Bắt đầu lưu vào DB
            string orderId = "ORD" + DateTime.Now.Ticks;
            decimal totalAmount = cart.Sum(i => i.Total);
            // TODO: Gọi DatabaseService để thực hiện:
            // 1. INSERT INTO Orders (id, user_id, order_status, total_amount, order_date)
            // 2. Duyệt vòng lặp INSERT INTO Order_items cho từng món trong giỏ
            foreach (var item in cart)
            {

            }
                // 3. INSERT INTO Payments (id, order_id, pay_method, pay_status)

                // Xóa giỏ hàng sau khi đặt thành công
                HttpContext.Session.Remove("Cart");

            return RedirectToAction("NotifySuccess");
        }

        public IActionResult NotifySuccess()
        {
            return View("NotifyPayment");
        }
    }
}
