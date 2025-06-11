namespace QuanLyKhoHang.Models
{
    public class StockIn
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
        public ICollection<StockInDetail> StockInDetails { get; set; } = new List<StockInDetail>();
    }
}
