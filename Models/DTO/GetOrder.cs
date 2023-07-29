namespace Loza.Models.DTO
{
    public class GetOrder
    {
        public int number { get; set; }
        public string shippingadress { get; set; }
        public int paymentmethod { get; set; }
        public DateTime orderdate { get; set; }
        public List<OrderItems> products { get; set; }
    }
}
