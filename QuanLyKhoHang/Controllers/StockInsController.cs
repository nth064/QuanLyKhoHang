using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoHang.Models;

namespace QuanLyKhoHang.Controllers
{
    public class StockInsController : Controller
    {
        private readonly ApplicationDBContext _context;

        public StockInsController(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var stockIns = await _context.StockIns
                .Include(s => s.Supplier)
                .Include(s => s.StockInDetails)
                    .ThenInclude(d => d.Product)
                .ToListAsync();
            return View(stockIns);
        }

        public IActionResult Create()
        {
            var suppliers = _context.Suppliers
                .Select(s => new {
                    s.Id,
                    Display = s.Name + " (ID: " + s.Id + ")"
                }).ToList();
            ViewBag.SupplierList = new SelectList(suppliers, "Id", "Display");
            ViewBag.Products = _context.Products.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(StockIn stockIn, List<StockInDetail> details)
        {
            if (ModelState.IsValid)
            {
                _context.StockIns.Add(stockIn);
                await _context.SaveChangesAsync();

                foreach (var item in details)
                {
                    item.StockInId = stockIn.Id;
                    _context.StockInDetails.Add(item);

                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        product.Quantity += item.Quantity;
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Nếu model không hợp lệ, gán lại ViewBag.SupplierList và ViewBag.Products trước khi trả view
            var suppliers = _context.Suppliers
                .Select(s => new {
                    s.Id,
                    Display = s.Name + " (ID: " + s.Id + ")"
                }).ToList();
            ViewBag.SupplierList = new SelectList(suppliers, "Id", "Display");
            ViewBag.Products = _context.Products.ToList();

            return View(stockIn);
        }


        public IActionResult Edit(int id)
        {
            var stockIn = _context.StockIns
                .Include(s => s.StockInDetails)
                .FirstOrDefault(s => s.Id == id);
            if (stockIn == null)
            {
                return NotFound();
            }
            ViewBag.SupplierList = new SelectList(_context.Suppliers, "Id", "Name", stockIn.SupplierId);
            ViewBag.Products = _context.Products.ToList();
            return View(stockIn);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StockIn stockIn, List<StockInDetail> details)
        {
            if (id != stockIn.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var existingStockIn = await _context.StockIns
                    .Include(s => s.StockInDetails)
                    .FirstOrDefaultAsync(s => s.Id == id);
                if (existingStockIn == null)
                {
                    return NotFound();
                }
                // Cập nhật thông tin StockIn
                existingStockIn.SupplierId = stockIn.SupplierId;
                existingStockIn.Date = stockIn.Date;
                // Xóa chi tiết cũ
                _context.StockInDetails.RemoveRange(existingStockIn.StockInDetails);
                // Thêm chi tiết mới
                foreach (var item in details)
                {
                    item.StockInId = existingStockIn.Id;
                    _context.StockInDetails.Add(item);
                    // Cập nhật số lượng sản phẩm
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        product.Quantity += item.Quantity;
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(stockIn);
        }
        public async Task<IActionResult> Details(int id)
        {
            var stockIn = await _context.StockIns
                .Include(s => s.Supplier)
                .Include(s => s.StockInDetails)
                    .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (stockIn == null)
            {
                return NotFound();
            }
            return View(stockIn);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var stockIn = await _context.StockIns
                .Include(s => s.Supplier)
                .Include(s => s.StockInDetails)
                    .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (stockIn == null)
            {
                return NotFound();
            }
            return View(stockIn);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stockIn = await _context.StockIns
                .Include(s => s.StockInDetails)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (stockIn != null)
            {
                // Trừ lại tồn kho
                foreach (var detail in stockIn.StockInDetails)
                {
                    var product = await _context.Products.FindAsync(detail.ProductId);
                    if (product != null)
                    {
                        product.Quantity -= detail.Quantity;
                    }
                }

                // Xóa chi tiết trước
                _context.StockInDetails.RemoveRange(stockIn.StockInDetails);
                _context.StockIns.Remove(stockIn);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
