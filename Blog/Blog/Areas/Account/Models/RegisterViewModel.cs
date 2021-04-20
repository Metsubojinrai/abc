using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Areas.Account.Models
{
    public class RegisterViewModel
    {
        [BindProperty]
        public InputModel Input { get; set; }
        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Phải nhập tên người dùng.")]
            [StringLength(100, ErrorMessage = "{0} dài từ {2} đến {1} ký tự.", MinimumLength = 4)]
            [DataType(DataType.Text)]
            [Display(Name = "Tên tài khoản (viết liền - không dấu)")]
            public string UserName { set; get; }

            [Required(ErrorMessage = "Phải nhập Email.")]
            [EmailAddress]
            [Display(Name = "Địa chỉ Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Phải nhập mật khẩu.")]
            [StringLength(100, ErrorMessage = "{0} dài từ {2} đến {1} ký tự.", MinimumLength = 4)]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Nhập lại mật khẩu")]
            [Compare("Password", ErrorMessage = "Mật khẩu không giống nhau")]
            public string ConfirmPassword { get; set; }
        }

    }
}
