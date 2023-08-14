namespace Loza.Models.DTO
{
    public class GetAllOrders
    {
        public int orderNumber { get; set; }
        public string useraddress { get; set; }
        public bool? isDelivered { get; set; }
        public DateTime Orderdate { get; set; }
        
    } 
}
