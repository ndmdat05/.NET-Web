using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult Index()
        {
            // Khi vào /Blog, hiển thị file Blog.cshtml
            return View("Blog");
        }

        public IActionResult Detail(int id)
        {
            // Khi vào chi tiết, hiển thị file Blog-detail.cshtml
            return View("BlogDetail");
        }
    }
 }