using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Data;
using WebBanHang.Models;

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
        [HttpPost]
        public async Task<IActionResult> ExportOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                // Chuyển sang Status 2: Đang giao hàng
                order.Status = 2;

                // (Tùy chọn) Ghi nhận thời gian xuất kho
                // order.ShippedDate = DateTime.Now; 

                await _context.SaveChangesAsync();
                TempData["Success"] = "Đơn hàng #" + id + " đã xuất kho thành công!";
            }

            // Sau khi xong, nhảy sang trang Shipping để theo dõi
            return RedirectToAction(nameof(Shipping));
        }

        // Hàm lấy danh sách đơn đang đi trên đường (Status = 2)
        public async Task<IActionResult> Shipping()
        {
            var orders = await _context.Orders
                .Where(o => o.Status == 2)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }
        [HttpPost]
        public async Task<IActionResult> CompleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                order.Status = 3; // 3: Giao thành công / Hoàn tất
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Shipping));
        }
        // 1. Trang danh sách lịch sử (Status = 3)
        public async Task<IActionResult> History()
        {
            var history = await _context.Orders
                .Where(o => o.Status == 3)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return View(history);
        }

        // 2. Trang xem chi tiết một đơn hàng bất kỳ
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null) return NotFound();

            return View(order);
        }
        
    }
}