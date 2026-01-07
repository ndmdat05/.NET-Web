using Microsoft.AspNetCore.Mvc;

namespace WebShop.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View("Cart");
        }

        public IActionResult Payment()
        {
            return View();
        }

        public IActionResult NotifyPayment()
        {
            return View();
        }
    }
}