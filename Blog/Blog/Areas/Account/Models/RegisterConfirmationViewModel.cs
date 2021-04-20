using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Areas.Account.Models
{
    public class RegisterConfirmationViewModel
    {
        public string Email { get; set; }

        public string UrlContinue { set; get; }
    }
}
