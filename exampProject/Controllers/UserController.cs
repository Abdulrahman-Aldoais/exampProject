using exampProject.DBContext;
using exampProject.Models;
using exampProject.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace exampProject.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly DbaceContext _dbaceContext;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _sinInManager;
        private readonly RoleManager<Role> _roleManager;
        public UserController(DbaceContext dbaceContext, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<Role> roleManager)
        {
            _dbaceContext = dbaceContext;
            _userManager = userManager;
            _sinInManager = signInManager;
            _roleManager = roleManager;
        }


        public async Task<IActionResult> AllUser()
        {
            var users = await _userManager.Users.ToListAsync();

            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            return View(userViewModels);
        }



        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.Roles = _dbaceContext.Roles.ToList();
            var model = new UserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = roles.FirstOrDefault(),

            };
            return PartialView("_EditUserPartial", model);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateUser(UserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return View(model);
            }

            user.Email = model.Email;
            user.Name = model.Name;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            // Role handling
            var roleExists = await _roleManager.RoleExistsAsync(model.Role);
            if (!roleExists)
            {
                ModelState.AddModelError("", "Role does not exist");
                return View(model);
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var roleDelegate = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            var roleResult = await _userManager.AddToRoleAsync(user, model.Role);
            if (!roleDelegate.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            return RedirectToAction("AllUser");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is UserController controller &&
                   EqualityComparer<RoleManager<Role>>.Default.Equals(_roleManager, controller._roleManager);
        }
    }
}
