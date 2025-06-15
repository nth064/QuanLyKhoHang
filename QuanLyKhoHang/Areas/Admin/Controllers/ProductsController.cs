using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoHang.Models;

namespace QuanLyKhoHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_StaffStockIn + "," + SD.Role_StaffStockOut)]
    public class ProductsController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(ApplicationDBContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Products - Tất cả role đều có thể xem
        public async Task<IActionResult> Index(string searchString, int? categoryId, int page = 1, int pageSize = 10)
        {
            // Lưu từ khóa và danh mục cho view
            ViewBag.SearchString = searchString;
            ViewBag.CategoryId = categoryId;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            // Lấy danh sách danh mục cho dropdown
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");

            var products = _context.Products.Include(p => p.Category).AsQueryable();

            // Tìm kiếm theo tên, mô tả, tên danh mục (không phân biệt hoa/thường)
            if (!string.IsNullOrEmpty(searchString))
            {
                var keyword = searchString.ToLower().Trim();
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

            // Sắp xếp theo ngày tạo mới nhất
            products = products.OrderByDescending(p => p.CreatedAt);

            // Phân trang
            var totalCount = await products.CountAsync();
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var pagedProducts = await products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return View(pagedProducts);
        }

        // GET: Products/Details/5 - Tất cả role đều có thể xem chi tiết
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create - Chỉ Admin có thể tạo mới
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Products/Create - Chỉ Admin có thể tạo mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Create([Bind("Name,Description,Price,Quantity,CategoryId,ImageFile")] Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Xử lý upload hình ảnh
                    if (product.ImageFile != null && product.ImageFile.Length > 0)
                    {
                        var imageUrl = await SaveImageAsync(product.ImageFile);
                        if (imageUrl == null)
                        {
                            ModelState.AddModelError("ImageFile", "Có lỗi xảy ra khi tải lên hình ảnh");
                            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                            return View(product);
                        }
                        product.ImageUrl = imageUrl;
                    }

                    // Set thời gian
                    product.CreatedAt = DateTime.Now;
                    product.UpdatedAt = DateTime.Now;

                    _context.Add(product);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                }
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5 - Chỉ Admin có thể chỉnh sửa
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5 - Chỉ Admin có thể chỉnh sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,Quantity,CategoryId,ImageFile,ImageUrl")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    // Xử lý upload hình ảnh mới
                    if (product.ImageFile != null && product.ImageFile.Length > 0)
                    {
                        // Xóa hình ảnh cũ
                        if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                        {
                            DeleteImage(existingProduct.ImageUrl);
                        }

                        // Lưu hình ảnh mới
                        var newImageUrl = await SaveImageAsync(product.ImageFile);
                        if (newImageUrl == null)
                        {
                            ModelState.AddModelError("ImageFile", "Có lỗi xảy ra khi tải lên hình ảnh");
                            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                            return View(product);
                        }
                        product.ImageUrl = newImageUrl;
                    }
                    else
                    {
                        // Giữ nguyên hình ảnh cũ nếu không upload hình mới
                        product.ImageUrl = existingProduct.ImageUrl;
                    }

                    // Preserve original CreatedAt
                    product.CreatedAt = existingProduct.CreatedAt;
                    product.UpdatedAt = DateTime.Now;

                    _context.Update(product);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                }
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5 - Chỉ Admin có thể xóa
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5 - Chỉ Admin có thể xóa
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product != null)
                {
                    // Xóa file hình ảnh nếu có
                    if (!string.IsNullOrEmpty(product.ImageUrl))
                    {
                        DeleteImage(product.ImageUrl);
                    }

                    _context.Products.Remove(product);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Xóa sản phẩm thành công!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa sản phẩm: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // Method để lưu hình ảnh
        private async Task<string?> SaveImageAsync(IFormFile imageFile)
        {
            try
            {
                // Kiểm tra định dạng file
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };
                var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("ImageFile", "Chỉ chấp nhận file hình ảnh (.jpg, .jpeg, .png, .gif, .webp, .bmp)");
                    return null;
                }

                // Kiểm tra kích thước file (5MB)
                if (imageFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("ImageFile", "File hình ảnh không được vượt quá 5MB");
                    return null;
                }

                // Tạo thư mục nếu chưa tồn tại
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Tạo tên file unique
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Lưu file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                // Trả về đường dẫn relative
                return "/images/products/" + uniqueFileName;
            }
            catch (Exception ex)
            {
                // Log error (nếu có logging service)
                ModelState.AddModelError("ImageFile", "Có lỗi xảy ra khi lưu hình ảnh: " + ex.Message);
                return null;
            }
        }

        // Method để xóa hình ảnh
        private void DeleteImage(string imageUrl)
        {
            try
            {
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error (nếu có logging service)
                // Không throw exception để không ảnh hưởng đến flow chính
                Console.WriteLine($"Error deleting image: {ex.Message}");
            }
        }

        // Helper method
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        // API endpoint để lấy sản phẩm theo danh mục (có thể dùng cho AJAX)
        [HttpGet]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var products = await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .Select(p => new { p.Id, p.Name, p.Price, p.Quantity })
                .ToListAsync();

            return Json(products);
        }

        // API endpoint để kiểm tra tên sản phẩm có trùng không
        [HttpGet]
        public async Task<IActionResult> CheckProductName(string name, int? id = null)
        {
            var exists = await _context.Products
                .AnyAsync(p => p.Name.ToLower() == name.ToLower() && (id == null || p.Id != id));

            return Json(!exists);
        }
    }
}