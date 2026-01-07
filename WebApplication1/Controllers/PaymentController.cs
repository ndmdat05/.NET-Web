using Microsoft.AspNetCore.Mvc;

namespace WebShop.Controllers
{
    public class PaymentController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NotifySuccess()
        {
            return View("NotifyPayment");
        }
    }
}
