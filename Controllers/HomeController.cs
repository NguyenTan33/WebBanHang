using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebBanHang.Data;
using WebBanHang.Models;
using WebBanHang.Services;

namespace WebBanHang.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RoleService _roleService;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, RoleService roleService, ApplicationDbContext context)
        {
            _logger = logger;
            _roleService = roleService;
            _context = context;
        }

        // ===== TRANG CHỦ + HIỂN THỊ SẢN PHẨM =====
        [AllowAnonymous]
        public async Task<IActionResult> Index(string search)
        {
            int role = _roleService.GetRole(User);
            ViewBag.Role = role;

            IQueryable<Product> query = _context.Products.Include(p => p.Category);

            if (!string.IsNullOrWhiteSpace(search))
            {
                string keyword = search.Trim().ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(keyword));
                ViewBag.SearchQuery = search;
            }

            var products = await query.ToListAsync();
            return View(products);
        }

        // ===== PRIVACY PAGE =====
        public IActionResult Privacy()
        {
            return View();
        }

        // ===== TRANG ADMIN =====
        public IActionResult AdminPage()
        {
            int role = _roleService.GetRole(User);
            if (role != 1)
                return RedirectToAction("AccessDenied");
            return View();
        }

        // ===== TRANG STAFF =====
        public IActionResult StaffPage()
        {
            int role = _roleService.GetRole(User);
            if (role != 1 && role != 2)
                return RedirectToAction("AccessDenied");
            return View();
        }

        // ===== KHÔNG QUYỀN TRUY CẬP =====
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return Content("Bạn không có quyền truy cập!");
        }

        // ===== ERROR PAGE =====
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}