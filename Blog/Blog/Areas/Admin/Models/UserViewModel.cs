using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Areas.Admin.Models
{
    public class UserViewModel
    {
        public List<UserInList> users;
        public int TotalPages { set; get; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { set; get; }
        public string ReturnUrl { get; set; }
    }

    public class UserInList : User
    {
        // Liệt kê các Role của User ví dụ: "Admin,Editor" ...
        public string ListRoles { set; get; }
    }
}
