using Microsoft.AspNetCore.Mvc;
using WebShop.Helpers;
using WebShop.Models;

namespace WebShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly string _connectionString =
           "Server=127.0.0.1;Database=DOCNET;User Id=root;Password=123456;Port=3306;";

        // 1. Đăng nhập
        // URL: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View(); // Tự động tìm Views/Account/Login.cshtml
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            // Xử lý đăng nhập ở đây (sau này)
            // Hiện tại chỉ giả lập thành công và chuyển hướng về trang chủ
            if(!ModelState.IsValid) return View(model);
            var conn = new MySql.Data.MySqlClient.MySqlConnection(_connectionString);
            conn.Open();
            string sql = "SELECT COUNT(*) FROM Users WHERE email=@username";
            using var cmd = new MySql.Data.MySqlClient.MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@username", model.Email);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read() || !BCrypt.Net.BCrypt.Verify(model.Password, reader["password"].ToString()))
            {
                ModelState.AddModelError(string.Empty, "Sai tên đăng nhập hoặc mật khẩu.");
                return View(model);
            }
            HttpContext.Session.SetInt32("UserId", Convert.ToInt32(reader["id"]));
            HttpContext.Session.SetString("UserEmail", reader["email"].ToString());
            return RedirectToAction("Index", "Home");
        }

        // 2. Đăng ký
        // URL: /Account/Register
        public IActionResult Register()
        {
            return View(); // Tự động tìm Views/Account/Register.cshtml
        }

        // 3. Quên mật khẩu
        // URL: /Account/ForgotPassword
        public IActionResult ForgotPassword()
        {
            return View(); // Tự động tìm Views/Account/ForgotPassword.cshtml
        }

        // 4. Thông báo gửi lại mật khẩu thành công
        // URL: /Account/ForgotSuccess
        public IActionResult ForgotSuccess()
        {

            return View(); // Tự động tìm Views/Account/ForgotSuccess.cshtml
        }

        // 5. Hồ sơ cá nhân
        // URL: /Account/Profile
        public IActionResult Profile()
        {
            return View(); // Tự động tìm Views/Account/Profile.cshtml
        }

        // 6. Danh sách yêu thích (Thêm mới cho đủ bộ)
        // URL: /Account/Wishlist
        public IActionResult Wishlist()
        {
            return View(); // Tự động tìm Views/Account/Wishlist.cshtml
        }

        // 7. Trạng thái đơn hàng (Danh sách đơn)
        // URL: /Account/ProductStatus 
        public IActionResult ProductStatus()
        {
            var orders = HttpContext.Session.Get<List<OrderViewModel>>("Orders");

            if (orders == null)
                orders = new List<OrderViewModel>(); // ✅ CHỐNG NULL

            return View(orders); // ✅ PHẢI TRUYỀN MODEL
        }


        // 8. Chi tiết đơn hàng
        // URL: /Account/StatusDetails/5
        public IActionResult StatusDetails(string id)
        {
            var items = HttpContext.Session.Get<List<OrderItem>>("OrderItems_" + id)
                        ?? new List<OrderItem>();

            return View(items); 
        }

                            // Sau này sẽ dùng id để lấy dữ liệu đơn hàng từ DB
             // Tự động tìm Views/Account/StatusDetails.cshtml
        

        // 9. Đăng xuất
        public IActionResult Logout()
        {
            // Xử lý xóa session/cookie ở đây (sau này)
            return RedirectToAction("Index", "Home"); // Quay về trang chủ
        }
    }
}