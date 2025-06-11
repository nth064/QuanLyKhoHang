using Microsoft.EntityFrameworkCore;
using QuanLyKhoHang.Models;

namespace QuanLyKhoHang.Repository
{
    public class EFCategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDBContext _context;
        public EFCategoryRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }
        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }
        public async Task AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var category = await GetByIdAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}
