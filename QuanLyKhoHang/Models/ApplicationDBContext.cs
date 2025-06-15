using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace QuanLyKhoHang.Models
{
    public class ApplicationDBContext : IdentityDbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
        }
        // Define DbSets for your entities here
         public DbSet<Product> Products { get; set; }
         public DbSet<Category> Categories { get; set; }
        // Add other DbSets as needed
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<StockIn> StockIns { get; set; }
        public DbSet<StockInDetail> StockInDetails { get; set; }
        public DbSet<StockOut> StockOuts { get; set; }
        public DbSet<StockOutDetail> StockOutDetails { get; set; }
    }
}
