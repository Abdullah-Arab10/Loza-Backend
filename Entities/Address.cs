using System.ComponentModel.DataAnnotations;

namespace Loza.Entities
{
    public class Address
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Location { get; set; }
        public string AddressName { get; set; }
    }
}
