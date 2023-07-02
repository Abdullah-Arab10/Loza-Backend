
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Loza.Entities   
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Wallet { get; set; } = 0;
        
        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public DateOnly DateOfBirth { get; set; }

    }
}
