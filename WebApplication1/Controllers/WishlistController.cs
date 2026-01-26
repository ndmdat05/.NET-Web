using Microsoft.AspNetCore.Mvc;
using WebShop.Models;
using WebShop.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace WebShop.Controllers
{
    public class WishlistController : Controller
    {
        private readonly DatabaseService _dbService;
        private const string WISHLIST_KEY = "Session_Wishlist";

        // Inject DatabaseService
        public WishlistController(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public IActionResult Index()
        {
            var wishlist = HttpContext.Session.Get<List<WishlistViewModel>>(WISHLIST_KEY) ?? new List<WishlistViewModel>();
            return View(wishlist);
        }

        [HttpPost]
        public IActionResult Toggle(string id) // QUAN TRỌNG: id phải là string
        {
            // 1. Lấy list từ Session
            var wishlist = HttpContext.Session.Get<List<WishlistViewModel>>(WISHLIST_KEY) ?? new List<WishlistViewModel>();

            // 2. Kiểm tra tồn tại
            var existingItem = wishlist.FirstOrDefault(x => x.ProductId == id);
            bool isLiked = false;

            if (existingItem != null)
            {
                // Có rồi -> Xóa
                wishlist.Remove(existingItem);
                isLiked = false;
            }
            else
            {
                // Chưa có -> Gọi DatabaseService lấy thông tin
                var product = _dbService.GetProductById(id);
                if (product != null)
                {
                    wishlist.Add(product);
                    isLiked = true;
                }
            }

            // 3. Lưu lại Session
            HttpContext.Session.Set(WISHLIST_KEY, wishlist);

            return Json(new { success = true, liked = isLiked });
        }
    }
}