using QuanLyKhoHang.Models;

namespace QuanLyKhoHang.Repository
{
    public interface IStockOutDetailRepository
    {
        Task<IEnumerable<StockOutDetail>> GetAllAsync();
        Task<StockOutDetail?> GetByIdAsync(int id);
        Task AddAsync(StockOutDetail stockOutDetail);
        Task UpdateAsync(StockOutDetail stockOutDetail);
        Task DeleteAsync(int id);
    }
}
