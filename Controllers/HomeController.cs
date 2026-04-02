using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebBanHang.Data;
using WebBanHang.Models;
using WebBanHang.Services;

namespace WebBanHang.Controllers
{
    [Authorize] // Yêu cầu đăng nhập mặc định cho toàn bộ Controller
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

        // ===== TRANG CHỦ + TÌM KIẾM SẢN PHẨM =====
        [AllowAnonymous] // Cho phép khách chưa đăng nhập vẫn xem được hàng
        public async Task<IActionResult> Index(string search, int? categoryId)
        {
            ViewBag.Role = _roleService.GetRole(User);
            IQueryable<Product> query = _context.Products.Include(p => p.Category);

            // Trường hợp 1: Nếu người dùng bấm vào nút "Lọc theo loại"
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
                ViewBag.CurrentCategory = categoryId.Value;
                // Xóa trắng SearchQuery ở View nếu muốn "lọc loại" đè lên "tìm kiếm"
                ViewBag.SearchQuery = null;
            }
            // Trường hợp 2: Nếu người dùng gõ ô Search (và không bấm nút loại)
            else if (!string.IsNullOrWhiteSpace(search))
            {
                string keyword = search.Trim();
                query = query.Where(p => p.Name.Contains(keyword));
                ViewBag.SearchQuery = keyword;
            }

            var products = await query.ToListAsync();
            return View(products);
        }

        // ===== TRANG CHÍNH SÁCH =====
        public IActionResult Privacy()
        {
            ViewBag.Role = _roleService.GetRole(User);
            return View();
        }

        // ===== TRANG ADMIN (Role 1) =====
        public IActionResult AdminPage()
        {
            int role = _roleService.GetRole(User);
            ViewBag.Role = role;

            if (role != 1)
                return RedirectToAction("AccessDenied");

            return View();
        }

        // ===== TRANG STAFF (Role 1 hoặc 2) =====
        public IActionResult StaffPage()
        {
            int role = _roleService.GetRole(User);
            ViewBag.Role = role;

            if (role != 1 && role != 2)
                return RedirectToAction("AccessDenied");

            return View();
        }

        // ===== TRANG THÔNG BÁO TỪ CHỐI TRUY CẬP =====
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View(); // Bạn nên tạo một View AccessDenied.cshtml cho đẹp thay vì Content()
        }

        // ===== TRANG LỖI HỆ THỐNG =====
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}