using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient; // Thêm thư viện MySQL
using System.Diagnostics;
using WebApplication1.Models;


namespace WebShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DatabaseService _databaseService; // Dùng Service thay vì MySqlConnection trực tiếp

        // Constructor nhận MySqlConnection và ILogger qua Dependency Injection
        public HomeController(ILogger<HomeController> logger, DatabaseService databaseService)
        {
            _logger = logger;
            _databaseService = databaseService;
        }

        public IActionResult Index()
        {
            string userId = User.Identity.Name ?? "Guest";
            ViewBag.WishlistIds = _databaseService.GetWishlistProductIds(userId);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // Các action khác vẫn giữ nguyên

        public IActionResult Blog()
        {
            return View();
        }

        public IActionResult BlogDetail()
        {
            return View();
        }

        public IActionResult DangKyTK()
        {
            return View();
        }

        public IActionResult DangNhap()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        // Kiểm tra kết nối MySQL
        public IActionResult TestDatabase()
        {
            // Gọi hàm TestConnection có sẵn trong DatabaseService.cs
            bool isConnected = _databaseService.TestConnection();

            if (isConnected)
            {
                return Content("Kết nối MySQL thành công!");
            }
            else
            {
                return Content("Kết nối MySQL thất bại . Kiểm tra lại Console log để xem chi tiết lỗi.");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}