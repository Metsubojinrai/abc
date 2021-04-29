using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class Order
    {
        public int Id { get; set; }

        public IEnumerable<OrderDetail> OrderLines { get; set; }
        [Display(Name = "Tổng tiền")]
        public decimal OrderTotal { get; set; }

        [Display(Name = "Ngày đặt")]
        public DateTime OrderPlaced { get; set; }

        public long UserId { get; set; }

        public User User { get; set; }
    }
}
