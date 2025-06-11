
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoHang.Models;

namespace QuanLyKhoHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductsController : Controller
    {
        private readonly ApplicationDBContext _context;

        public ProductsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: Products

        public async Task<IActionResult> Index(string searchString, int? categoryId)
        {
            // Lưu từ khóa và danh mục cho view
            ViewBag.SearchString = searchString;
            ViewBag.CategoryId = categoryId;

            // Lấy danh sách danh mục cho dropdown
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");

            var products = _context.Products.Include(p => p.Category).AsQueryable();

            // Tìm kiếm theo tên, mô tả, tên danh mục (không phân biệt hoa/thường, hỗ trợ tiếng Việt không dấu)
            if (!string.IsNullOrEmpty(searchString))
            {
                var keyword = searchString.ToLower();
                products = products.Where(p =>
                    p.Name.ToLower().Contains(keyword) ||
                    p.Description.ToLower().Contains(keyword) ||
                    p.Category.Name.ToLower().Contains(keyword)
                );
            }

            // Tìm kiếm theo danh mục
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }

            return View(await products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Price,Quantity,CategoryId")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.CreatedAt = DateTime.Now;
                product.UpdatedAt = DateTime.Now;
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,Quantity,CategoryId")] Product product)
        {
            if (id != product.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    if (existingProduct == null)
                        return NotFound();

                    product.CreatedAt = existingProduct.CreatedAt; // preserve original CreatedAt
                    product.UpdatedAt = DateTime.Now;

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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }


    }
}
