﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoHang.Models;
using QuanLyKhoHang.Repository;

namespace QuanLyKhoHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_StaffStockIn + "," + SD.Role_StaffStockOut)]
    public class SuppliersController : Controller
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly ApplicationDBContext _context;

        public SuppliersController(ISupplierRepository supplierRepository, ApplicationDBContext context)
        {
            _supplierRepository = supplierRepository;
            _context = context;
        }

        // GET: Supplier
        public async Task<IActionResult> Index(string searchString)
        {
            ViewBag.SearchString = searchString;

            var suppliers = await _supplierRepository.GetAllAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                var keyword = searchString.ToLower();
                suppliers = suppliers.Where(s =>
                    s.Name.ToLower().Contains(keyword) ||
                    s.Address.ToLower().Contains(keyword) ||
                    s.Phone.ToLower().Contains(keyword) ||
                    s.Email.ToLower().Contains(keyword)
                ).ToList();
            }

            return View(suppliers);
        }

        // GET: Supplier/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Supplier/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                await _supplierRepository.AddAsync(supplier);
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        // GET: Supplier/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        // POST: Supplier/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Supplier supplier)
        {
            if (id != supplier.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _supplierRepository.UpdateAsync(supplier);
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        // GET: Supplier/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        // POST: Supplier/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            // Kiểm tra nếu nhà cung cấp đã có phiếu nhập thì không cho xóa
            bool hasStockIns = await _context.StockIns.AnyAsync(s => s.SupplierId == id);
            if (hasStockIns)
            {
                ModelState.AddModelError(string.Empty, "Không thể xóa nhà cung cấp vì đã có phiếu nhập liên quan.");
                return View("Delete", supplier);
            }

            await _supplierRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Supplier/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }
    }
}
