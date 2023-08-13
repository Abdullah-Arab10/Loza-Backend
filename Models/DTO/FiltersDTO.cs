namespace Loza.Models.DTO
{
    public class FiltersDTO
    {
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public int? Categories { get; set; }
        public string? Color { get; set; }
        public int? ColorNo { get; set; }
    }
}
