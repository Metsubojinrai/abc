using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Areas.Account.Models
{
    [AllowAnonymous]
    public class LockoutViewModel
    {
        public void OnGet()
        {

        }
    }
}
