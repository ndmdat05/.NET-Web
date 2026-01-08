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
            return View("Payment");
        }

        public IActionResult NotifyPayment()
        {
            return View("NotifyPayment");
        }
    }
}