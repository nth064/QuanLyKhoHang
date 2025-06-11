using QuanLyKhoHang.Models;

namespace QuanLyKhoHang.Repository
{
    public interface IStockOutRepository
    {
        Task<IEnumerable<StockOut>> GetAllAsync();
        Task<StockOut?> GetByIdAsync(int id);
        Task AddAsync(StockOut stockOut);
        Task UpdateAsync(StockOut stockOut);
        Task DeleteAsync(int id);
    }
}
