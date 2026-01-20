using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using WebShop.Models;

[Area("Admin")]
public class OrdersController : Controller
{
    private readonly MySqlConnection _conn;
    public OrdersController(MySqlConnection conn) { _conn = conn; }

    public IActionResult Index()
    {
        List<OrderViewModel> list = new List<OrderViewModel>();
        if (_conn.State == ConnectionState.Closed) _conn.Open();

        // Join bảng Orders và Users để lấy tên người mua
        string sql = @"
            SELECT o.id, u.email, o.total_amount, o.order_status, o.order_date 
            FROM Orders o
            LEFT JOIN Users u ON o.user_id = u.id
            ORDER BY o.order_date DESC";

        using (var cmd = new MySqlCommand(sql, _conn))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                list.Add(new OrderViewModel
                {
                    Id = reader["id"].ToString(),
                    CustomerName = reader["email"].ToString(), // Tạm hiện email
                    TotalAmount = Convert.ToDecimal(reader["total_amount"]),
                    Status = reader["order_status"].ToString(),
                    OrderDate = Convert.ToDateTime(reader["order_date"])
                });
            }
        }
        _conn.Close();
        return View(list);
    }
}