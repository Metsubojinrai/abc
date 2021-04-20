using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Areas.Account.Models
{
    public class ConfirmEmailChangeViewModel
    {
        [TempData]
        public string StatusMessage { get; set; }
    }
}
