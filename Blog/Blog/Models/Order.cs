using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class Order
    {
        public int Id { get; set; }

        public IEnumerable<OrderDetail> OrderLines { get; set; }
        public decimal OrderTotal { get; set; }

        public DateTime OrderPlaced { get; set; }

        public long UserId { get; set; }

        public User User { get; set; }
    }
}
