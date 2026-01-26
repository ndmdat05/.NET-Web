using Microsoft.AspNetCore.Mvc;
using WebShop.Models;
using WebShop.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace WebShop.Controllers
{
    public class WishlistController : Controller
    {
        private const string WISHLIST_KEY = "WISHLIST";

        public IActionResult Index()
        {
            var wishlist = HttpContext.Session.Get<List<Wishlist>>(WISHLIST_KEY)
                           ?? new List<Wishlist>();

            return View(wishlist);
        }

        [HttpPost]
        public IActionResult Toggle(int id, string name, string image, decimal price)
        {
            var wishlist = HttpContext.Session.Get<List<Wishlist>>(WISHLIST_KEY)
                           ?? new List<Wishlist>();

            var existing = wishlist.FirstOrDefault(x => x.Id == id);

            if (existing != null)
            {
                wishlist.Remove(existing);
                HttpContext.Session.Set(WISHLIST_KEY, wishlist);
                return Json(new { liked = false });
            }

            wishlist.Add(new Wishlist
            {
                Id = id,
                Name = name,
                Image = image,
                Price = price
            });

            HttpContext.Session.Set(WISHLIST_KEY, wishlist);
            return Json(new { liked = true });
        }
    }
}
