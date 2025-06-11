namespace QuanLyKhoHang.Models
{
    public class StockOutDetail
    {
        public int Id { get; set; }
        public int StockOutId { get; set; }
        public StockOut? StockOut { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

}
