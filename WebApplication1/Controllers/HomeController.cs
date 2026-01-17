using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient; // Thêm thư viện MySQL
using System.Diagnostics;
using WebApplication1.Models;


namespace WebShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MySqlConnection _connection; // Inject MySqlConnection từ Services

        // Constructor nhận MySqlConnection và ILogger qua Dependency Injection
        public HomeController(ILogger<HomeController> logger, MySqlConnection connection)
        {
            _logger = logger;
            _connection = connection;
        }

        public IActionResult Index()
        {
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
            try
            {
                _connection.Open(); 
                _connection.Close(); 
                return Content("Ket noi mySQL thanh cong! ✅");
            }
            catch (Exception ex)
            {
                return Content($"ket noi mySQL that bai: {ex.Message} ❌");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}