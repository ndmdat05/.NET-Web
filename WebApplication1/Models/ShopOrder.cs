using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Models
{
    public class ShopOrder
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }

        public List<OrderItem> Items { get; set; } = new();
    }
}

