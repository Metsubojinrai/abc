using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Areas.Admin.Models
{
    public class RoleViewModel
    {
        public long Id { get; set; }
        [Display(Name = "Tên Role")]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
