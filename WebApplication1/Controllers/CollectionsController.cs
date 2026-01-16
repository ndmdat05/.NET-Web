using Microsoft.AspNetCore.Mvc;

namespace WebShop.Controllers
{
    public class CollectionsController : Controller
    {
        public IActionResult PateMeoCon()
        {
            // Dùng chung file giao diện collections.cshtml cho danh mục này
            return View("collections");
        }
    }
}