using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    [Table("Product")]
    public class Product
    {
        [Key]
        public int ProductId { set; get; }

        [Display(Name = "Tên sản phẩm")]
        public string Name { set; get; }
        [Display(Name = "Mô tả")]
        public string Description { set; get; }
        [Display(Name = "Giá")]
        public decimal Price { set; get; }

        [Display(Name = "Ngày tạo")]
        public DateTime DateCreated { set; get; }

        [Display(Name = "Ngày cập nhật")]
        public DateTime DateUpdated { set; get; }
        [Display(Name ="Ảnh sản phẩm")]
        public string ProductPicture { get; set; }

        public List<ProductCategory> ProductCategories { set; get; }
    }
}
