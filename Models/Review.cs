using System.ComponentModel.DataAnnotations;
using Loza.Entities;
using Loza.Models.User;

namespace LozaApi.Models
{
    public class Review
    {
        [Key]
        public string Id { get; set; }

        public string? Rreviews { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }


        public int UserId { get; set; }
        
    }
}
