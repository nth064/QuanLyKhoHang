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
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_StaffStockOut)]
    public class StockOutDetailsController : Controller
    {
        private readonly ApplicationDBContext _context;

        public StockOutDetailsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: StockOutDetails

        public async Task<IActionResult> Index(string searchString)
        {
            ViewBag.SearchString = searchString;

            var details = _context.StockOutDetails
                .Include(d => d.Product)
                .Include(d => d.StockOut)
                    .ThenInclude(s => s.Customer)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                var keyword = searchString.ToLower();
                details = details.Where(d =>
                    d.Product.Name.ToLower().Contains(keyword) ||
                    d.StockOut.Customer.Name.ToLower().Contains(keyword) ||
                    (d.Quantity * d.Price).ToString().Contains(keyword)
                );
            }

            return View(await details.ToListAsync());
        }

        // GET: StockOutDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockOutDetail = await _context.StockOutDetails
                .Include(s => s.Product)
                .Include(s => s.StockOut)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stockOutDetail == null)
            {
                return NotFound();
            }

            return View(stockOutDetail);
        }

        // GET: StockOutDetails/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id");
            ViewData["StockOutId"] = new SelectList(_context.StockOuts, "Id", "Id");
            return View();
        }

        // POST: StockOutDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StockOutId,ProductId,Quantity,Price")] StockOutDetail stockOutDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(stockOutDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", stockOutDetail.ProductId);
            ViewData["StockOutId"] = new SelectList(_context.StockOuts, "Id", "Id", stockOutDetail.StockOutId);
            return View(stockOutDetail);
        }

        // GET: StockOutDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockOutDetail = await _context.StockOutDetails.FindAsync(id);
            if (stockOutDetail == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", stockOutDetail.ProductId);
            ViewData["StockOutId"] = new SelectList(_context.StockOuts, "Id", "Id", stockOutDetail.StockOutId);
            return View(stockOutDetail);
        }

        // POST: StockOutDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StockOutId,ProductId,Quantity,Price")] StockOutDetail stockOutDetail)
        {
            if (id != stockOutDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stockOutDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockOutDetailExists(stockOutDetail.Id))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", stockOutDetail.ProductId);
            ViewData["StockOutId"] = new SelectList(_context.StockOuts, "Id", "Id", stockOutDetail.StockOutId);
            return View(stockOutDetail);
        }

        // GET: StockOutDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockOutDetail = await _context.StockOutDetails
                .Include(s => s.Product)
                .Include(s => s.StockOut)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stockOutDetail == null)
            {
                return NotFound();
            }

            return View(stockOutDetail);
        }

        // POST: StockOutDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stockOutDetail = await _context.StockOutDetails.FindAsync(id);
            if (stockOutDetail != null)
            {
                _context.StockOutDetails.Remove(stockOutDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StockOutDetailExists(int id)
        {
            return _context.StockOutDetails.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Print(int id)
        {
            var stockOut = await _context.StockOuts
                .Include(s => s.Customer)
                .Include(s => s.StockOutDetails)
                    .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (stockOut == null)
            {
                return NotFound();
            }

            return View(stockOut); 
        }

    }
}
