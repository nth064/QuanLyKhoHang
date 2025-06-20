﻿namespace QuanLyKhoHang.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public ICollection<StockIn> StockIns { get; set; } = new List<StockIn>();
    }
}
