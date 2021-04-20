using Blog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Areas.Admin.Models
{
    public class ClaimsViewModel
    {
        public Role role { set; get; }
        public IList<IdentityRoleClaim<long>> claims { get; set; }
        public IdentityRoleClaim<long> claim { get; set; }
    }
}
