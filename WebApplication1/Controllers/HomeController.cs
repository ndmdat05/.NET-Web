using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient; // Thêm thư viện MySQL
using System.Data.Common;
using System.Diagnostics;
using WebApplication1.Models;
using WebShop.Models;


namespace WebShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly DatabaseService _databaseService;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, DatabaseService databaseService)
        {
            _logger = logger;
            _configuration = configuration;
            _databaseService = databaseService;
        }

        //Lấy chuỗi kết nối
        private string GetConnectionString() => _configuration.GetConnectionString("DefaultConnection");

        //Hàm xử lý dữ liệu
        private HomeViewModel LoadHomeData()
        {
            var model = new HomeViewModel();

            using (var conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                //Sp giảm giá
                string sqlDiscount = @"
                    SELECT p.id, p.name, p.price, p.sale_price, pi.image_url
                    FROM Products p
                    LEFT JOIN Product_Images pi ON p.id = pi.product_id AND pi.is_main = 1
                    WHERE p.sale_price > 0 AND p.sale_price < p.price
                    ORDER BY (p.price - p.sale_price) DESC
                    LIMIT 8";

                using (var cmd = new MySqlCommand(sqlDiscount, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.DiscountedProducts.Add(new ProductViewModel
                        {
                            Id = reader["id"].ToString(),
                            Name = reader["name"].ToString(),
                            Price = Convert.ToDecimal(reader["price"]),
                            SalePrice = reader["sale_price"] != DBNull.Value ? Convert.ToDecimal(reader["sale_price"]) : null,
                            ImageUrl = reader["image_url"] != DBNull.Value ? reader["image_url"].ToString() : "/images/default.png"
                        });
                    }
                }

                //Sp đề xuất
                string sqlRecommend = @"
                    SELECT p.id, p.name, p.price, p.sale_price, pi.image_url
                    FROM Products p
                    LEFT JOIN Product_Images pi ON p.id = pi.product_id AND pi.is_main = 1
                    ORDER BY RAND() 
                    LIMIT 8";

                using (var cmd = new MySqlCommand(sqlRecommend, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.RecommendedProducts.Add(new ProductViewModel
                        {
                            Id = reader["id"].ToString(),
                            Name = reader["name"].ToString(),
                            Price = Convert.ToDecimal(reader["price"]),
                            SalePrice = reader["sale_price"] != DBNull.Value ? Convert.ToDecimal(reader["sale_price"]) : null,
                            ImageUrl = reader["image_url"] != DBNull.Value ? reader["image_url"].ToString() : "/images/default.png"
                        });
                    }
                }
            }
            return model;
        }

        public IActionResult Index()
        {
            string userId = User.Identity.Name ?? "Guest";
            ViewBag.WishlistIds = _databaseService.GetWishlistProductIds(userId);
            var model = LoadHomeData();
            return View(model);
        }

        public IActionResult IndexAfterLogin()
        {
            var model = LoadHomeData();
            return View(model);
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