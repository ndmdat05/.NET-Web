using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using WebShop.Models;

namespace WebShop.Controllers
{
    public class ProductController : Controller
    {
       
        // Giả sử Action bạn đặt tên là Detail hoặc ProductDetail
        public IActionResult Detail(int id)
        {
            // Ép hệ thống dùng file ProductDetail.cshtml thay vì Detail.cshtml
            return View("ProductDetail");
        }
       
    

        public IActionResult SearchResult(string q)
        {
            return View("SearchResult"); // Hoặc return View() cũng được vì tên khớp
        }
    }
}
