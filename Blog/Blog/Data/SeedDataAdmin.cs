using Blog.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Data
{
    public static class SeedDataAdmin
    {
        public static void SeedAdmin(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            User user = new()
            {
                UserName = "admin",
                Email = "admin@gmail.com",
                EmailConfirmed = true
            };

            if (userManager.FindByEmailAsync("admin@gmail.com").Result == null)
            {
               var urs = userManager.CreateAsync(user, "123456").Result;
            }

            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                var role = new Role()
                {
                    Name = "Admin",
                    Description = "Boss"
                };
                var rs = roleManager.CreateAsync(role).Result;
                if (rs.Succeeded)
                {
                    userManager.AddToRoleAsync(user, role.Name).Wait();
                }
            }
        }
    }
}
