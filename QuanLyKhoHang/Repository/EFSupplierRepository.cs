using Microsoft.EntityFrameworkCore;
using QuanLyKhoHang.Models;

namespace QuanLyKhoHang.Repository
{
    public class EFSupplierRepository : ISupplierRepository
    {
        private readonly ApplicationDBContext _context;
        public EFSupplierRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            return await _context.Suppliers.ToListAsync();
        }
        public async Task<Supplier?> GetByIdAsync(int id)
        {
            return await _context.Suppliers.FindAsync(id);
        }
        public async Task AddAsync(Supplier supplier)
        {
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Supplier supplier)
        {
            _context.Suppliers.Update(supplier);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var supplier = await GetByIdAsync(id);
            if (supplier != null)
            {
                _context.Suppliers.Remove(supplier);
                await _context.SaveChangesAsync();
            }
        }
    }
}
