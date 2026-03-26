using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebBanHang.Models;
using WebBanHang.Services;

namespace WebBanHang.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RoleService _roleService;

        public HomeController(ILogger<HomeController> logger, RoleService roleService)
        {
            _logger = logger;
            _roleService = roleService;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            int role = _roleService.GetRole(User);

            ViewBag.Role = role;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult AdminPage()
        {
            int role = _roleService.GetRole(User);

            if (role != 1)
                return RedirectToAction("AccessDenied");

            return View();
        }

        // 🔥 ADMIN + STAFF (role = 1 hoặc 2)
        public IActionResult StaffPage()
        {
            int role = _roleService.GetRole(User);

            if (role != 1 && role != 2)
                return RedirectToAction("AccessDenied");

            return View();
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return Content("Bạn không có quyền truy cập!");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}