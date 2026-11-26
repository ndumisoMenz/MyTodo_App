using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Todo_Application.Models;

namespace Todo_Application.Controllers
{
    [Authorize(Roles ="Manager")]
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index(string searchString,int pageNumber = 1)
        {
            var usersQuery = _userManager.Users.AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                usersQuery = usersQuery.Where(u => u.UserName.Contains(searchString));
            }

            int pageSize = 2;
            int totalCount = await usersQuery.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var pagedUsers = await usersQuery.OrderBy(u => u.UserName).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var model = new List<UserViewModel>();

            foreach (var user in pagedUsers)
            {
                var role= await _userManager.GetRolesAsync(user);

                model.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    Role = string.Join(",", role)
                });
            }

            ViewBag.Pagination = new PaginationInfo
            {
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                SearchString = searchString,
                StatusFilter = "",
                ActionName = "Index",
                ControllerName = "Users"
            };
            return View(model);
        }

        public IActionResult Add() 
        {
            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> add(string email,string password,string role)
        {
            var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded) 
            {
                await _userManager.AddToRoleAsync(user, role);
                return RedirectToAction("Index");
            }

            foreach(var err in result.Errors)
            {
                ModelState.AddModelError("", err.Description);
            }
            return View(result); 
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id) 
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }
    }
}
