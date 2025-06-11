using Microsoft.EntityFrameworkCore;
using QuanLyKhoHang.Models;

namespace QuanLyKhoHang.Repository
{
    public class EFStockOutRepository : IStockOutRepository
    {
        private readonly ApplicationDBContext _context;
        public EFStockOutRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<StockOut>> GetAllAsync()
        {
            return await _context.StockOuts.ToListAsync();
        }
        public async Task<StockOut?> GetByIdAsync(int id)
        {
            return await _context.StockOuts.FindAsync(id);
        }
        public async Task AddAsync(StockOut stockOut)
        {
            _context.StockOuts.Add(stockOut);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(StockOut stockOut)
        {
            _context.StockOuts.Update(stockOut);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var stockOut = await GetByIdAsync(id);
            if (stockOut != null)
            {
                _context.StockOuts.Remove(stockOut);
                await _context.SaveChangesAsync();
            }
        }
    }
}
