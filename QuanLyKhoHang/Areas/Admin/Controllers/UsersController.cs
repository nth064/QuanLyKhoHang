using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoHang.Models;

namespace QuanLyKhoHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_StaffStockOut)]
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Admin/Users
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            // Dictionary để lưu role của từng user
            var userRolesDict = new Dictionary<string, IList<string>>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRolesDict[user.Id] = roles;
            }

            ViewBag.AllRoles = allRoles;
            ViewBag.UserRolesDict = userRolesDict;

            return View(users);
        }

        // GET: Admin/Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.Roles = roles;
            return View(user);
        }

        // GET: Admin/Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(user);

            ViewBag.AllRoles = allRoles;
            ViewBag.UserRoles = userRoles;

            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, IdentityUser model, string[] selectedRoles)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            selectedRoles = selectedRoles ?? new string[] { };

            var resultRemove = await _userManager.RemoveFromRolesAsync(user, userRoles);
            if (!resultRemove.Succeeded)
            {
                ModelState.AddModelError("", "Không thể xóa vai trò cũ.");
                return View(user);
            }

            var resultAdd = await _userManager.AddToRolesAsync(user, selectedRoles);
            if (!resultAdd.Succeeded)
            {
                ModelState.AddModelError("", "Không thể thêm vai trò mới.");
                return View(user);
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: Admin/Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        // POST: Admin/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
