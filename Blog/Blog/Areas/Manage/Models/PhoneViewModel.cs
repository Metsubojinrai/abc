using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Areas.Manage.Models
{
    public class PhoneViewModel
    {
        public string PhoneNumber { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required, Display(Name = "Code")]
            public string VerificationCode { get; set; }
        }
        
    }
}
