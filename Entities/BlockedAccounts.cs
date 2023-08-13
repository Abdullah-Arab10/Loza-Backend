using System.ComponentModel.DataAnnotations;

namespace Loza.Entities
{
    public class BlockedAccounts
    {
        [Key]
        public int Id { get; set; }
        [EmailAddress]
        public string Email { get; set; }
    }
}
