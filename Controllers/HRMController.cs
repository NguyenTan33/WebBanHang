using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebBanHang.Controllers
{
    public class HRMController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public HRMController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        private int IsAdmin()
        {
            int role = Convert.ToInt32(ViewBag.Role ?? 0);
            return role;
        }

        // GET: Danh sách nhân sự
        public async Task<IActionResult> Index()
        {
            if (IsAdmin()==1) return RedirectToAction("Index", "Home");

            var users = await _userManager.Users.ToListAsync();
            // Truyền thêm danh sách Roles qua ViewBag để làm Dropdown gán quyền cho nhanh
            ViewBag.AllRoles = await _roleManager.Roles.ToListAsync();

            return View(users);
        }

        // POST: Gán quyền cho User (Tác động bảng nối AspNetUserRoles)
        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            if (IsAdmin()==1) return RedirectToAction("Index", "Home");

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && !string.IsNullOrEmpty(roleName))
            {
                // Xóa các quyền cũ của User này trước khi gán quyền mới (tránh trùng lặp)
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

                // Thêm quyền mới vào bảng nối
                await _userManager.AddToRoleAsync(user, roleName);
                TempData["Success"] = $"Đã gán quyền {roleName} cho {user.UserName}";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Xóa nhân sự
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (IsAdmin() == 1) return RedirectToAction("Index", "Home");

            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
                TempData["Success"] = "Đã xóa tài khoản nhân sự thành công!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}