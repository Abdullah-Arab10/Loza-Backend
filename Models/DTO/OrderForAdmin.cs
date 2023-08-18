namespace Loza.Models.DTO
{
    public class OrderForAdmin
    {
        public int RequestNumber { get; set; }
        public int OrderNumber { get; set; }
        public string shippingadress { get; set; }
        public int paymentmethod { get; set; }
        public DateTime orderdate { get; set; }
        public bool? isDelivered { get; set; }
        public string phonenumber { get; set; }
        public string username { get; set; }
        public decimal TotalCheck { get; set; }
        public List<OrderItems> products { get; set; }
    }
}
