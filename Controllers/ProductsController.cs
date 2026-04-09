using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanHang.Data;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Kiểm tra role helper
        private int IsAdmin()
        {
                int role = Convert.ToInt32(ViewBag.Role ?? 0);
            return role;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            if (IsAdmin()== 1)
                return RedirectToAction("Index", "Home"); // hoặc trả về NotFound/Unauthorized
            return View(await _context.Products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (IsAdmin()==1)
                return RedirectToAction("Index", "Home");

            if (id == null)
                return NotFound();

            var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            // 1. Kiểm tra quyền Admin trước
            // Vì IsAdmin() trả về true/false nên chỉ cần viết thế này:
            if (IsAdmin() == 1)
            {
                // Nếu KHÔNG PHẢI Admin, đuổi ngay về trang chủ
                return RedirectToAction("Index", "Home");
            }

            // 2. Nếu là Admin, chuẩn bị dữ liệu cho Dropdown phân loại
            // Lấy Id để lưu vào database, lấy Name để hiện chữ (Thịt, Cá, Trứng, Sữa...)
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");

            // 3. Trả về trang Thêm sản phẩm
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Amount,Image")] Product product)
        {
            if (IsAdmin()==1)
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // 1. Kiểm tra quyền (IsAdmin trả về bool nên dùng dấu "!" để phủ định)
            if (IsAdmin()==1)
            {
                // Nếu KHÔNG PHẢI Admin thì "mời" ra trang chủ
                return RedirectToAction("Index", "Home");
            }

            // 2. Kiểm tra ID đầu vào
            if (id == null) return NotFound();

            // 3. Tìm sản phẩm trong Database
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            // 4. QUAN TRỌNG: Nạp danh sách Category để ô chọn (Thịt, Cá, Sữa...) hiển thị được
            // Tham số cuối 'product.CategoryId' giúp nó tự động chọn đúng loại của sản phẩm đó
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);

            return View(product);
        }

        

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (IsAdmin() == 1)
                return RedirectToAction("Index", "Home");

            if (id == null)
                return NotFound();

            var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (IsAdmin() == 1)
                return RedirectToAction("Index", "Home");

            var product = await _context.Products.FindAsync(id);
            if (product != null)
                _context.Products.Remove(product);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}