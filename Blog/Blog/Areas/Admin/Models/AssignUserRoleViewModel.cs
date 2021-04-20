using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Blog.Areas.Admin.Models
{
    public class AssignUserRoleViewModel
    {
        public class InputModel
        {
            [Required]
            public string ID { set; get; }
            public string Name { set; get; }

            public string[] RoleNames { set; get; }

        }

        [BindProperty]
        public InputModel Input { set; get; }

        [BindProperty]
        public bool IsConfirmed { set; get; }

        public List<string> AllRoles { set; get; } = new List<string>();
    }
}
