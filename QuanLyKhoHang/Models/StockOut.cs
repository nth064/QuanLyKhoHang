namespace QuanLyKhoHang.Models
{
    public class StockOut
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public ICollection<StockOutDetail> StockOutDetails { get; set; } = new List<StockOutDetail>();
    }

}
