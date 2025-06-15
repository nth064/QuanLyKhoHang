using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoHang.Models;

namespace QuanLyKhoHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class DashboardController : Controller
    {
        private readonly ApplicationDBContext _context;

        public DashboardController(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var totalProducts = await _context.Products.CountAsync();
            var totalSuppliers = await _context.Suppliers.CountAsync();
            var totalCustomers = await _context.Customers.CountAsync();
            var totalStockIns = await _context.StockIns.CountAsync();
            var totalStockOuts = await _context.StockOuts.CountAsync();

            var totalStockInQty = await _context.StockInDetails.SumAsync(s => (int?)s.Quantity) ?? 0;
            var totalStockOutQty = await _context.StockOutDetails.SumAsync(s => (int?)s.Quantity) ?? 0;
            var totalStockQuantity = totalStockInQty - totalStockOutQty;

            var totalRevenue = await _context.StockOutDetails
                .SumAsync(s => (decimal?)s.Quantity * s.Price) ?? 0;

            var model = new StatisticsViewModel
            {
                TotalProducts = totalProducts,
                TotalSuppliers = totalSuppliers,
                TotalCustomers = totalCustomers,
                TotalStockIns = totalStockIns,
                TotalStockOuts = totalStockOuts,
                TotalStockQuantity = totalStockQuantity,
                TotalRevenue = totalRevenue
            };

            return View(model);
        }
    }
}
