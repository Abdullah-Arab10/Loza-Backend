using System.ComponentModel.DataAnnotations;

namespace Loza.Models.DTOs
{
    public class UserUpdateRequestDto
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public DateOnly? DateOfBirth { get; set; }
    }
}
