using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Data;
using WebBanHang.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace WebBanHang.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Trang hiển thị giỏ hàng (Đã fix lỗi Model null)
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            // Nếu chưa có giỏ hàng, tạo một object rỗng gửi sang View để không bị lỗi foreach
            if (cart == null)
            {
                cart = new Cart { UserId = userId, CartItems = new List<CartItem>() };
            }

            return View(cart);
        }

        // 2. Thêm đồ vào giỏ (Dùng AJAX - Đã thêm IgnoreAntiforgeryToken để demo mượt)
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> AddToCart(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId);

            if (cartItem == null)
            {
                _context.CartItems.Add(new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = 1
                });
            }
            else
            {
                cartItem.Quantity++;
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đã thêm vào giỏ hàng của quý khách!" });
        }

        // 3. Tăng/Giảm số lượng (Dùng cho trang Index giỏ hàng)
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int amount)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                cartItem.Quantity += amount;
                if (cartItem.Quantity <= 0)
                {
                    _context.CartItems.Remove(cartItem);
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // 4. Xóa sản phẩm khỏi giỏ
        [HttpPost]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // Trang nhập thông tin giao hàng và xác nhận
        [HttpGet]
        public async Task<IActionResult> ConfirmOrder()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any()) return RedirectToAction("Index");

            return View(cart);
        }

        // Chỉnh lại hàm Checkout để nhận thông tin từ Form
        [HttpPost]
        public async Task<IActionResult> Checkout(string fullName, string phone, string address)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null && cart.CartItems.Any())
            {
                // Ở đây m có thể log thông tin ra Console để demo cho thầy xem
                Console.WriteLine($"Khách hàng: {fullName} - SĐT: {phone} - ĐC: {address}");

                foreach (var item in cart.CartItems)
                {
                    if (item.Product != null)
                    {
                        item.Product.Amount -= item.Quantity; // Trừ kho thật
                    }
                }

                _context.CartItems.RemoveRange(cart.CartItems); // Xóa giỏ
                await _context.SaveChangesAsync();

                return RedirectToAction("CheckoutSuccess");
            }

            return RedirectToAction("Index");
        }
        // 5. Thanh toán (Trừ kho + Xóa giỏ - Cú chốt demo cho thầy)
        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null && cart.CartItems.Any())
            {
                foreach (var item in cart.CartItems)
                {
                    if (item.Product != null)
                    {
                        // Trừ số lượng tồn kho của sản phẩm
                        item.Product.Amount -= item.Quantity;
                        if (item.Product.Amount < 0) item.Product.Amount = 0;
                    }
                }

                // Xóa các món đồ trong giỏ sau khi thanh toán
                _context.CartItems.RemoveRange(cart.CartItems);
                await _context.SaveChangesAsync();

                return RedirectToAction("CheckoutSuccess");
            }

            return RedirectToAction("Index");
        }

        // Trang báo thành công
        public IActionResult CheckoutSuccess()
        {
            return View();
        }
    }
}