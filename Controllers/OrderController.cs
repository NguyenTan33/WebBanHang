using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Data;

namespace WebBanHang.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Bước 1: Hiện danh sách đơn hàng mới (Status = 0)
        public async Task<IActionResult> Index()
        {
            // Chỉ lấy các đơn hàng mới đặt
            var newOrders = await _context.Orders
                .Where(o => o.Status == 0)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(newOrders);
        }

        // Bước 2: Hàm "Nhận đơn" - Chuyển đơn sang trạng thái Soạn hàng (Status = 1)
        [HttpPost]
        public async Task<IActionResult> ReceiveOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                order.Status = 1; // Chuyển sang trạng thái "Đang soạn hàng"
                await _context.SaveChangesAsync();

                // Sau khi nhận đơn, nhảy thẳng vào trang Soạn hàng (Checklist)
                return RedirectToAction("PrepareList", new { id = order.Id });
            }
            return RedirectToAction(nameof(Index));
        }
        // Trang danh sách đơn đang chờ soạn hàng (Status = 1)
        public async Task<IActionResult> PrepareList()
        {
            var processingOrders = await _context.Orders
                .Where(o => o.Status == 1) // Lọc những ông đang soạn hàng
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(processingOrders);
        }
        // GET: Orders/Process/5
        public async Task<IActionResult> Process(int id)
        {
            // Lấy đơn hàng kèm theo chi tiết món hàng và thông tin sản phẩm
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}