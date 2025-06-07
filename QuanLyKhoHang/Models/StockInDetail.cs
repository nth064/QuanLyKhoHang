namespace QuanLyKhoHang.Models
{
    public class StockInDetail
    {
        public int Id { get; set; }
        public int StockInId { get; set; }
        public StockIn? StockIn { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

}
