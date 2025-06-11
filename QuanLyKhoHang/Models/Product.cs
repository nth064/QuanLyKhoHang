namespace QuanLyKhoHang.Models
{
    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

    }

}
