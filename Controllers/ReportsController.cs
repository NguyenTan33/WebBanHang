using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Data;

namespace WebBanHang.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var thisMonth = new DateTime(today.Year, today.Month, 1);

            // 1. Tổng doanh thu (Tất cả đơn Status = 3)
            // Fix lỗi lồng SUM bằng cách đi từ OrderDetails
            ViewBag.TotalRevenue = await _context.OrderDetails
                .Where(d => d.Order.Status == 3)
                .SumAsync(d => d.Price * d.Quantity);

            // 2. Doanh thu hôm nay
            ViewBag.TodayRevenue = await _context.OrderDetails
                .Where(d => d.Order.Status == 3 && d.Order.OrderDate >= today)
                .SumAsync(d => d.Price * d.Quantity);

            // 3. Doanh thu tháng này
            ViewBag.MonthRevenue = await _context.OrderDetails
                .Where(d => d.Order.Status == 3 && d.Order.OrderDate >= thisMonth)
                .SumAsync(d => d.Price * d.Quantity);

            // 4. Tổng số đơn hàng thành công
            ViewBag.OrderCount = await _context.Orders.CountAsync(o => o.Status == 3);

            // 5. Chuẩn bị dữ liệu biểu đồ 7 ngày qua
            var last7Days = Enumerable.Range(0, 7)
                .Select(i => DateTime.Today.AddDays(-i))
                .OrderBy(d => d).ToList();

            var chartLabels = new List<string>();
            var chartData = new List<decimal>();

            foreach (var date in last7Days)
            {
                chartLabels.Add(date.ToString("dd/MM"));

                // Lấy khoảng thời gian trong ngày (từ 00:00 đến 23:59)
                var nextDay = date.AddDays(1);
                var daySum = await _context.OrderDetails
                    .Where(d => d.Order.Status == 3 &&
                           d.Order.OrderDate >= date &&
                           d.Order.OrderDate < nextDay)
                    .SumAsync(d => d.Price * d.Quantity);

                chartData.Add(daySum);
            }

            ViewBag.ChartLabels = chartLabels;
            ViewBag.ChartData = chartData;

            return View();
        }
    }
}