using Microsoft.EntityFrameworkCore;
using QuanLyKhoHang.Models;

namespace QuanLyKhoHang.Repository
{
    public class EFStockOutDetailRepository : IStockOutDetailRepository
    {
        private readonly ApplicationDBContext _context;
        public EFStockOutDetailRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<StockOutDetail>> GetAllAsync()
        {
            return await _context.StockOutDetails.ToListAsync();
        }
        public async Task<StockOutDetail?> GetByIdAsync(int id)
        {
            return await _context.StockOutDetails.FindAsync(id);
        }
        public async Task AddAsync(StockOutDetail stockOutDetail)
        {
            _context.StockOutDetails.Add(stockOutDetail);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(StockOutDetail stockOutDetail)
        {
            _context.StockOutDetails.Update(stockOutDetail);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var stockOutDetail = await GetByIdAsync(id);
            if (stockOutDetail != null)
            {
                _context.StockOutDetails.Remove(stockOutDetail);
                await _context.SaveChangesAsync();
            }

        }
    }
}
