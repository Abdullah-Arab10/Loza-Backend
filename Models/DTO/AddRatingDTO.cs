namespace Loza.Models.DTO
{
    public class AddRatingDTO
    {
        public int userId { get; set; }
        public int productId { get; set; }
        public decimal rating { get; set; }
        public string? reviews { get; set; }
    }
}
