using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoHang.Models;

namespace QuanLyKhoHang.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDBContext _context;

        public HomeController(ApplicationDBContext context)
        {
            _context = context;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // 👉 Thêm trang hiển thị danh sách sản phẩm
        public IActionResult Index(int? categoryId)
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.SelectedCategory = categoryId;

            var products = _context.Products
                .Include(p => p.Category)
                .Where(p => !categoryId.HasValue || p.CategoryId == categoryId)
                .ToList();

            return View(products);
        }

    }
}
