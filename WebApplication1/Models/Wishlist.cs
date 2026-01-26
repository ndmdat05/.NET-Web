using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;

namespace WebShop.Models
{
    
        public class Wishlist
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Image { get; set; }
            public decimal Price { get; set; }
        }
    }

