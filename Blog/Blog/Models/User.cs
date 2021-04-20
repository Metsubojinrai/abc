using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class User : IdentityUser<long>
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override long Id { get; set; }
        public string FullName { set; get; }
        public string Address { set; get; }
        public DateTime? Birthday { set; get; }
        public string ProfilePicture { get; set; }
    }

    public class Role : IdentityRole<long>
    {
        public string Description { get; set; }
    }

    public class UserRole : IdentityUserRole<long>
    {

    }
}
