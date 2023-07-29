namespace Loza.Models.DTO
{
    public class OrderQuery
    {
      public int userid { get; set; }
        public int paymentmethod { get; set; }=2;
        public int addressid { get; set; }
        public decimal total { get; set; }

    }
}