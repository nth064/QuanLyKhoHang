using System;
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
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_StaffStockIn)]
    public class StockInDetailsController : Controller
    {
        private readonly ApplicationDBContext _context;

        public StockInDetailsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: StockInDetails

        public async Task<IActionResult> Index(string searchString)
        {
            ViewBag.SearchString = searchString;

            var details = _context.StockInDetails
                .Include(d => d.Product)
                .Include(d => d.StockIn)
                    .ThenInclude(s => s.Supplier)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                var keyword = searchString.ToLower();
                details = details.Where(d =>
                    d.Product.Name.ToLower().Contains(keyword) ||
                    d.StockIn.Supplier.Name.ToLower().Contains(keyword) ||
                    (d.Quantity * d.Price).ToString().Contains(keyword)
                );
            }

            return View(await details.ToListAsync());
        }


        // GET: StockInDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockInDetail = await _context.StockInDetails
                .Include(s => s.Product)
                .Include(s => s.StockIn)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stockInDetail == null)
            {
                return NotFound();
            }

            return View(stockInDetail);
        }

        // GET: StockInDetails/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id");
            ViewData["StockInId"] = new SelectList(_context.StockIns, "Id", "Id");
            return View();
        }

        // POST: StockInDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StockInId,ProductId,Quantity,Price")] StockInDetail stockInDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(stockInDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", stockInDetail.ProductId);
            ViewData["StockInId"] = new SelectList(_context.StockIns, "Id", "Id", stockInDetail.StockInId);
            return View(stockInDetail);
        }

        // GET: StockInDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockInDetail = await _context.StockInDetails.FindAsync(id);
            if (stockInDetail == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", stockInDetail.ProductId);
            ViewData["StockInId"] = new SelectList(_context.StockIns, "Id", "Id", stockInDetail.StockInId);
            return View(stockInDetail);
        }

        // POST: StockInDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StockInId,ProductId,Quantity,Price")] StockInDetail stockInDetail)
        {
            if (id != stockInDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stockInDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockInDetailExists(stockInDetail.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", stockInDetail.ProductId);
            ViewData["StockInId"] = new SelectList(_context.StockIns, "Id", "Id", stockInDetail.StockInId);
            return View(stockInDetail);
        }

        // GET: StockInDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockInDetail = await _context.StockInDetails
                .Include(s => s.Product)
                .Include(s => s.StockIn)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stockInDetail == null)
            {
                return NotFound();
            }

            return View(stockInDetail);
        }

        // POST: StockInDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stockInDetail = await _context.StockInDetails.FindAsync(id);
            if (stockInDetail != null)
            {
                _context.StockInDetails.Remove(stockInDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StockInDetailExists(int id)
        {
            return _context.StockInDetails.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Print(int id)
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


    }
}
