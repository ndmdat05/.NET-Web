using Microsoft.AspNetCore.Mvc;

namespace WebShop.Controllers
{
    public class AccountController : Controller
    {
        // 1. Đăng nhập
        // URL: /Account/Login
        public IActionResult Login()
        {
            return View(); // Tự động tìm Views/Account/Login.cshtml
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
            return View(); // Tự động tìm Views/Account/ProductStatus.cshtml
        }

        // 8. Chi tiết đơn hàng
        // URL: /Account/StatusDetails/5
        public IActionResult StatusDetails(int id)
        {
            // Sau này sẽ dùng id để lấy dữ liệu đơn hàng từ DB
            return View(); // Tự động tìm Views/Account/StatusDetails.cshtml
        }

        // 9. Đăng xuất
        public IActionResult Logout()
        {
            // Xử lý xóa session/cookie ở đây (sau này)
            return RedirectToAction("Index", "Home"); // Quay về trang chủ
        }
    }
}