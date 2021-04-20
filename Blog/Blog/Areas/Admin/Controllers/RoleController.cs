using Blog.Areas.Admin.Models;
using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("[controller]/[action]")]
    [Authorize(Roles="Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly BlogDbContext _dbContext;
        const int USER_PER_PAGE = 10;
        public RoleController(RoleManager<Role> roleManager, UserManager<User> userManager,
            BlogDbContext dbContext)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<RoleListViewModel> Roles = new();
            Roles = (List<RoleListViewModel>)_roleManager.Roles.Select(r => new RoleListViewModel
            {
                Name = r.Name,
                Id = r.Id,
                Description = r.Description
            }).ToList();
            return View(Roles);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(Role role)
        {
            if(ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                else AddErrors(result);
            }
            return View(role);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            RoleViewModel model = new();
            if(!String.IsNullOrEmpty(id))
            {
                var role = await _roleManager.FindByIdAsync(id);
                if(role!=null)
                {
                    model.Id = role.Id;
                    model.Name = role.Name;
                    model.Description = role.Description;
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(string id, RoleViewModel model)
        {
            if(ModelState.IsValid)
            {
                bool isExist = !String.IsNullOrEmpty(id);
                if (!isExist)
                    return NotFound();
                var role = await _roleManager.FindByIdAsync(id);
                role.Name = model.Name;
                role.Description = model.Description;
                _dbContext.Update(role);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                    return RedirectToAction(nameof(Index));
                else AddErrors(result);
            }
            else ModelState.AddModelError("", "Không tìm thấy Role");
            return View("Index", _roleManager.Roles);
        }

        public async Task<IActionResult> ListUser(UserViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var cuser = await _userManager.GetUserAsync(User);

            if (model.PageNumber == 0)
                model.PageNumber = 1;

            var lusers = (from u in _userManager.Users
                          orderby u.UserName
                          select new UserInList()
                          {
                              Id = u.Id,
                              UserName = u.UserName,
                          });


            int totalUsers = await lusers.CountAsync();

            model.TotalPages = (int)Math.Ceiling((double)totalUsers / USER_PER_PAGE);

            model.users = await lusers.Skip(USER_PER_PAGE * (model.PageNumber - 1)).Take(USER_PER_PAGE).ToListAsync();

            foreach (var user in model.users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.ListRoles = string.Join(",", roles.ToList());
            }

            return View(model);
        }

        public async Task<IActionResult> AssignUserRole(string id)
        {
            var model = new AssignUserRoleViewModel
            {
                Input = new AssignUserRoleViewModel.InputModel
                {
                    ID = id
                }
            };
            var allroles = await _roleManager.Roles.ToListAsync();

            allroles.ForEach((r) => { model.AllRoles.Add(r.Name); });
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignUserRole(AssignUserRoleViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Input.ID);
            if (user == null)
                return NotFound("Không tìm thấy user");

            var roles = await _userManager.GetRolesAsync(user);
            var allroles = await _roleManager.Roles.ToListAsync();

            allroles.ForEach((r) => { model.AllRoles.Add(r.Name); });
            if (model.IsConfirmed)
            {
                model.Input.RoleNames = roles.ToArray();
                model.IsConfirmed = true;
                ModelState.Clear();
            }
            else
            {
                if (model.Input.RoleNames == null) model.Input.RoleNames = Array.Empty<string>();
                foreach (var rolename in model.Input.RoleNames)
                {
                    if (roles.Contains(rolename)) continue;
                    await _userManager.AddToRoleAsync(user, rolename);
                }
                foreach (var rolename in roles)
                {
                    if (model.Input.RoleNames.Contains(rolename)) continue;
                    await _userManager.RemoveFromRoleAsync(user, rolename);
                }
                return RedirectToAction(nameof(ListUser));
            }

            model.Input.Name = user.UserName;
            return View(model);
        }

    }
}
