using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoHang.Models;

namespace QuanLyKhoHang.Controllers
{
    public class StockOutsController : Controller
    {
        private readonly ApplicationDBContext _context;

        public StockOutsController(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var stockOuts = await _context.StockOuts
                .Include(o => o.Customer)
                .Include(o => o.StockOutDetails)
                    .ThenInclude(d => d.Product)
                .ToListAsync();
            return View(stockOuts);
        }

        public IActionResult Create()
        {
            ViewBag.Customers = _context.Customers.ToList();
            ViewBag.Products = _context.Products.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StockOut stockOut, List<StockOutDetail> details)
        {
            // 1. Kiểm tra dữ liệu chi tiết
            foreach (var item in details)
            {
                if (item.Quantity <= 0)
                    ModelState.AddModelError("", "Số lượng sản phẩm phải lớn hơn 0.");
                if (item.Price <= 0)
                    ModelState.AddModelError("", "Giá xuất phải lớn hơn 0.");
            }

            // 2. Kiểm tra tồn kho
            foreach (var item in details)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                {
                    ModelState.AddModelError("", $"Sản phẩm ID {item.ProductId} không tồn tại.");
                    break;
                }
                if (product.Quantity < item.Quantity)
                {
                    ModelState.AddModelError("", $"Sản phẩm {product.Name} chỉ còn {product.Quantity} trong kho, không đủ để xuất {item.Quantity}.");
                    break;
                }
            }

            // 3. Nếu có lỗi thì trả lại view
            if (!ModelState.IsValid)
            {
                ViewBag.Customers = _context.Customers.ToList();
                ViewBag.Products = _context.Products.ToList();
                return View(stockOut);
            }

            // 4. Lưu đơn xuất
            _context.StockOuts.Add(stockOut);
            await _context.SaveChangesAsync();

            foreach (var item in details)
            {
                item.StockOutId = stockOut.Id;
                _context.StockOutDetails.Add(item);

                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.Quantity -= item.Quantity;
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var stockOut = await _context.StockOuts
                .Include(o => o.Customer)
                .Include(o => o.StockOutDetails)
                    .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (stockOut == null) return NotFound();

            return View(stockOut);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var stockOut = await _context.StockOuts
                .Include(o => o.StockOutDetails)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (stockOut == null) return NotFound();

            ViewBag.Customers = new SelectList(_context.Customers, "Id", "Name", stockOut.CustomerId);
            ViewBag.Products = _context.Products.ToList();

            return View(stockOut);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StockOut stockOut, List<StockOutDetail> details)
        {
            if (id != stockOut.Id) return NotFound();

            foreach (var item in details)
            {
                if (item.Quantity <= 0)
                    ModelState.AddModelError("", "Số lượng sản phẩm phải lớn hơn 0.");
                if (item.Price <= 0)
                    ModelState.AddModelError("", "Giá xuất phải lớn hơn 0.");
            }

            foreach (var item in details)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                {
                    ModelState.AddModelError("", $"Sản phẩm ID {item.ProductId} không tồn tại.");
                    break;
                }
                if (product.Quantity < item.Quantity)
                {
                    ModelState.AddModelError("", $"Sản phẩm {product.Name} chỉ còn {product.Quantity} trong kho, không đủ để xuất {item.Quantity}.");
                    break;
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Customers = _context.Customers.ToList();
                ViewBag.Products = _context.Products.ToList();
                return View(stockOut);
            }

            _context.Update(stockOut);
            await _context.SaveChangesAsync();

            // Xử lý lại chi tiết
            var oldDetails = _context.StockOutDetails.Where(d => d.StockOutId == stockOut.Id);
            _context.StockOutDetails.RemoveRange(oldDetails);

            foreach (var item in details)
            {
                item.StockOutId = stockOut.Id;
                _context.StockOutDetails.Add(item);

                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.Quantity -= item.Quantity;
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var stockOut = await _context.StockOuts
                .Include(s => s.StockOutDetails)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (stockOut == null) return NotFound();

            return View(stockOut);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stockOut = await _context.StockOuts
                .Include(s => s.StockOutDetails)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (stockOut != null)
            {
                foreach (var detail in stockOut.StockOutDetails)
                {
                    var product = await _context.Products.FindAsync(detail.ProductId);
                    if (product != null)
                    {
                        product.Quantity += detail.Quantity;
                    }
                }

                _context.StockOutDetails.RemoveRange(stockOut.StockOutDetails);
                _context.StockOuts.Remove(stockOut);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
