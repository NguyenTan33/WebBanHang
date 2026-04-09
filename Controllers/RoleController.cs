using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebBanHang.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // Helper check quyền Admin
        private int IsAdmin()
        {
            int role = Convert.ToInt32(ViewBag.Role ?? 0);
            return role;
        }


        // GET: Hiển thị danh sách các quyền
        public async Task<IActionResult> Index()
        {
            if (IsAdmin()==1) return RedirectToAction("Index", "Home");
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        // POST: Tạo quyền mới (Nhanh gọn lẹ)
        [HttpPost]
        public async Task<IActionResult> Create(string roleName)
        {
            if (IsAdmin()==1) return RedirectToAction("Index", "Home");
            if (!string.IsNullOrEmpty(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName.Trim()));
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Xóa quyền
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (IsAdmin()==1) return RedirectToAction("Index", "Home");
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null) await _roleManager.DeleteAsync(role);
            return RedirectToAction(nameof(Index));
        }
    }
}