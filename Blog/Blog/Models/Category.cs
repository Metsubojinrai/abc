using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    [Table("Category")] // Model  tương ứng với bảng Category
    public class Category
    {
        [Key]
        public int Id { get; set; }

        // Tên Category
        [Required(ErrorMessage = "Phải có tên loại sản phẩm")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} dài {2} đến {1}")]
        [Display(Name = "Tên loại sản phẩm")]
        public string Name { get; set; }

        // Mô tả về Category
        [Display(Name = "Mô tả")]
        public string Description { set; get; }

        public List<ProductCategory> ProductCategories { get; set; }
    }
}
