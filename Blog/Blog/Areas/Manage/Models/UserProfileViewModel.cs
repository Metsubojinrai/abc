using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Areas.Manage.Models
{
    public class UserProfileViewModel
    {
        [Display(Name ="Tên tài khoản")]
        public string UserName { get; set; }

        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [RegularExpression(@"(84|0[3|5|7|8|9])+([0-9]{8})\b", ErrorMessage = "Số điện thoại không đúng")]
            [Display(Name = "Số điện thoại")]
            public string PhoneNumber { get; set; }

            [MaxLength(100)]
            [Display(Name = "Họ tên đầy đủ")]
            public string FullName { set; get; }

            [MaxLength(255)]
            [Display(Name = "Địa chỉ")]
            public string Address { set; get; }

            [DataType(DataType.Date)]
            [Display(Name = "Ngày sinh d/m/y")]
            public DateTime? Birthday { set; get; }

            [Display(Name = "Avatar")]
            public string ProfilePicture { get; set; }
        }
    }
}
