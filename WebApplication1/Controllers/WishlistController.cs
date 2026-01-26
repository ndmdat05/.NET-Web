using Microsoft.AspNetCore.Mvc;
using WebShop.Models;
using WebShop.Helpers; // Dùng SessionExtensions bạn đã có
using System.Collections.Generic;
using System.Linq;

namespace WebShop.Controllers
{
    public class WishlistController : Controller
    {
        private readonly DatabaseService _dbService;
        private const string WISHLIST_KEY = "Session_Wishlist";

        public WishlistController(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public IActionResult Index()
        {
            // Lấy danh sách từ Session
            var wishlist = HttpContext.Session.Get<List<WishlistViewModel>>(WISHLIST_KEY) ?? new List<WishlistViewModel>();
            return View(wishlist);
        }

        [HttpPost]
        public IActionResult Toggle(string id)
        {
            // 1. Lấy danh sách hiện tại từ Session
            var wishlist = HttpContext.Session.Get<List<WishlistViewModel>>(WISHLIST_KEY) ?? new List<WishlistViewModel>();

            // 2. Kiểm tra xem sản phẩm đã có trong list chưa
            var existingItem = wishlist.FirstOrDefault(x => x.ProductId == id);

            bool isLiked = false;

            if (existingItem != null)
            {
                // Có rồi -> Xóa đi (Bỏ thích)
                wishlist.Remove(existingItem);
                isLiked = false;
            }
            else
            {
                // Chưa có -> Tìm trong DB để lấy thông tin -> Thêm vào List
                var product = _dbService.GetProductById(id);
                if (product != null)
                {
                    wishlist.Add(product);
                    isLiked = true;
                }
            }

            // 3. Lưu ngược lại vào Session
            HttpContext.Session.Set(WISHLIST_KEY, wishlist);

            // Trả về kết quả cho JS
            return Json(new { success = true, liked = isLiked });
        }
    }
}