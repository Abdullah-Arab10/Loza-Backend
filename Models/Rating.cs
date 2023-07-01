using System.ComponentModel.DataAnnotations;
using System.Numerics;
using Loza.Entities;
using Loza.Models.User;

namespace LozaApi.Models
{
    public class Rating
    {
        [Key]
        public Guid Id { get; set; }



        [Range(1, 5)]
        public decimal Rate { get; set; } 
        public string? Rreviews { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }


        public int UserId { get; set; }
        
    }
}
