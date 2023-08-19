﻿namespace Loza.Models.DTO
{
    public class Most5ProductsSold
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Category { get; set; }
        public string Color { get; set; }
        public long ColorNo { get; set; }
        public decimal Quantity { get; set; }
        public string? ProductImage { get; set; }
        public string? ProductDimensions { get; set; }
        public int TotalSales { get; set; }
    }
}
