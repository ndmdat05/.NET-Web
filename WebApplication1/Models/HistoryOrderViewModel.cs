using System;

namespace WebShop.Models
{
    public class HistoryOrderViewModel
    {
        public string OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
    }
}