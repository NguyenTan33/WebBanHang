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

<<<<<<< HEAD
        // ===== TRANG CHỦ + TÌM KIẾM SẢN PHẨM =====
        [AllowAnonymous] // Cho phép khách chưa đăng nhập vẫn xem được hàng
        public async Task<IActionResult> Index(string search)
        {
            // Luôn gán Role để Layout hiển thị đúng các nút chức năng
            ViewBag.Role = _roleService.GetRole(User);

            // Khởi tạo truy vấn lấy sản phẩm kèm theo Danh mục (Category)
            IQueryable<Product> query = _context.Products.Include(p => p.Category);

            // Xử lý logic tìm kiếm
            if (!string.IsNullOrWhiteSpace(search))
            {
                string keyword = search.Trim();
                // Tìm kiếm theo tên sản phẩm (EF Core tự xử lý không phân biệt hoa thường tùy DB)
                query = query.Where(p => p.Name.Contains(keyword));

                // Gửi lại từ khóa ra View để hiển thị thông báo "Kết quả tìm kiếm cho..."
                ViewBag.SearchQuery = keyword;
=======
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
>>>>>>> 005d5c558f352313f478a8b1e51f51dd5d8934a2
            }

            var products = await query.ToListAsync();
            return View(products);
        }

<<<<<<< HEAD
        // ===== TRANG CHÍNH SÁCH =====
=======
        // ===== PRIVACY PAGE =====
>>>>>>> 005d5c558f352313f478a8b1e51f51dd5d8934a2
        public IActionResult Privacy()
        {
            ViewBag.Role = _roleService.GetRole(User);
            return View();
        }

<<<<<<< HEAD
        // ===== TRANG ADMIN (Role 1) =====
        public IActionResult AdminPage()
        {
            int role = _roleService.GetRole(User);
            ViewBag.Role = role;

=======
        // ===== TRANG ADMIN =====
        public IActionResult AdminPage()
        {
            int role = _roleService.GetRole(User);
>>>>>>> 005d5c558f352313f478a8b1e51f51dd5d8934a2
            if (role != 1)
                return RedirectToAction("AccessDenied");
            return View();
        }

<<<<<<< HEAD
        // ===== TRANG STAFF (Role 1 hoặc 2) =====
        public IActionResult StaffPage()
        {
            int role = _roleService.GetRole(User);
            ViewBag.Role = role;

=======
        // ===== TRANG STAFF =====
        public IActionResult StaffPage()
        {
            int role = _roleService.GetRole(User);
>>>>>>> 005d5c558f352313f478a8b1e51f51dd5d8934a2
            if (role != 1 && role != 2)
                return RedirectToAction("AccessDenied");
            return View();
        }

<<<<<<< HEAD
        // ===== TRANG THÔNG BÁO TỪ CHỐI TRUY CẬP =====
=======
        // ===== KHÔNG QUYỀN TRUY CẬP =====
>>>>>>> 005d5c558f352313f478a8b1e51f51dd5d8934a2
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View(); // Bạn nên tạo một View AccessDenied.cshtml cho đẹp thay vì Content()
        }

<<<<<<< HEAD
        // ===== TRANG LỖI HỆ THỐNG =====
=======
        // ===== ERROR PAGE =====
>>>>>>> 005d5c558f352313f478a8b1e51f51dd5d8934a2
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}