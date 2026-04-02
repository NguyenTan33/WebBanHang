using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private bool IsAdmin()
        {
            int role = Convert.ToInt32(HttpContext.Session.GetInt32("Role") ?? 0);
            return role == 1;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
                return RedirectToAction("Index", "Home"); // hoặc trả về NotFound/Unauthorized
            return View(await _context.Products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsAdmin())
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
            if (!IsAdmin())
                return RedirectToAction("Index", "Home");
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Amount,Image")] Product product)
        {
            if (!IsAdmin())
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
            if (!IsAdmin())
                return RedirectToAction("Index", "Home");

            if (id == null)
                return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Amount,Image")] Product product)
        {
            if (!IsAdmin())
                return RedirectToAction("Index", "Home");

            if (id != product.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsAdmin())
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
            if (!IsAdmin())
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