using System.ComponentModel.DataAnnotations.Schema;

namespace Loza.Models.User
{
    public class UserGetById
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        [NotMapped]
        public DateOnly DateOfBirth { get; set; }
    }
}
