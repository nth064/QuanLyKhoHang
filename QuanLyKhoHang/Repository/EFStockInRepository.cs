using Microsoft.EntityFrameworkCore;
using QuanLyKhoHang.Models;

namespace QuanLyKhoHang.Repository
{
    public class EFStockInRepository : IStockInRepository
    {
        private readonly ApplicationDBContext _context;
        public EFStockInRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<StockIn>> GetAllAsync()
        {
            return await _context.StockIns.ToListAsync();
        }
        public async Task<StockIn?> GetByIdAsync(int id)
        {
            return await _context.StockIns.FindAsync(id);
        }
        public async Task AddAsync(StockIn stockIn)
        {
            _context.StockIns.Add(stockIn);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(StockIn stockIn)
        {
            _context.StockIns.Update(stockIn);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var stockIn = await GetByIdAsync(id);
            if (stockIn != null)
            {
                _context.StockIns.Remove(stockIn);
                await _context.SaveChangesAsync();
            }
        }
    }
}
