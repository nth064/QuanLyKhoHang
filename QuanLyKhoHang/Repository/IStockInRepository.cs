using QuanLyKhoHang.Models;

namespace QuanLyKhoHang.Repository
{
    public interface IStockInRepository
    {
        Task<IEnumerable<StockIn>> GetAllAsync();
        Task<StockIn?> GetByIdAsync(int id);
        Task AddAsync(StockIn stockIn);
        Task UpdateAsync(StockIn stockIn);
        Task DeleteAsync(int id);
    }
}
