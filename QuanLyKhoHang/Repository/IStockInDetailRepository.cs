using QuanLyKhoHang.Models;

namespace QuanLyKhoHang.Repository
{
    public interface IStockInDetailRepository
    {
        Task<IEnumerable<StockInDetail>> GetAllAsync();
        Task<StockInDetail?> GetByIdAsync(int id);
        Task AddAsync(StockInDetail stockInDetail);
        Task UpdateAsync(StockInDetail stockInDetail);
        Task DeleteAsync(int id);
    }
}
