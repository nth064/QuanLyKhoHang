using Microsoft.EntityFrameworkCore;
using QuanLyKhoHang.Models;

namespace QuanLyKhoHang.Repository
{
    public class EFStockInDetailRepository : IStockInDetailRepository
    {
        private readonly ApplicationDBContext _context;
        public EFStockInDetailRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<StockInDetail>> GetAllAsync()
        {
            return await _context.StockInDetails.ToListAsync();
        }
        public async Task<StockInDetail?> GetByIdAsync(int id)
        {
            return await _context.StockInDetails.FindAsync(id);
        }
        public async Task AddAsync(StockInDetail stockInDetail)
        {
            _context.StockInDetails.Add(stockInDetail);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(StockInDetail stockInDetail)
        {
            _context.StockInDetails.Update(stockInDetail);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var stockInDetail = await GetByIdAsync(id);
            if (stockInDetail != null)
            {
                _context.StockInDetails.Remove(stockInDetail);
                await _context.SaveChangesAsync();
            }

        }
    }
}
