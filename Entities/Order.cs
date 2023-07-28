using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loza.Entities
{
    public class Order
    {
        [Key]
        public int Order_Id { get; set; }
        public int User_Id { get; set; }
        public string? User_Adress { get; set; }
        public bool? Deleverd { get; set; } = false;
        public int paymethod { get; set; } 
        public int AdressId { get; set; }
        [DefaultValue("GETDATE()")]
        public DateTime Created_at { get; set; } = DateTime.Now.AddSeconds(-DateTime.Now.Second);


    }
}
