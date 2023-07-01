namespace LozaApi.Models.DTO
{
    public class ProductsDTO 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Category{ get; set; }
        public string Color { get; set; }
        public decimal Quantity { get; set; }
        public string? ProductImage { get; set; }
       
        // public List<string>? ProductImages { get; set; }
        // public DateTime CreateDateTime { get; set; }
        //  public DateTime UpdateDateTime { get; set; }



    }
}
