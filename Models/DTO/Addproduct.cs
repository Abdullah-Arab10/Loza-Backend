using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Loza.Models.DTO
{
    public class AddproductDTO
    {

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        [Range(0, 10000)]
        public decimal Price { get; set; } = 0;

        [Required]

        [Range(0, 7)]
        public int Category { get; set; }

        [Required]
        [StringLength(24)]
        public string Color { get; set; }

        public long ColorNo { get; set; }
        [Required]
        [Range(0, 100)]
        public decimal Quantity { get; set; } = 0;

        public string? ProductDimensions { get; set; }

        public string? ProductImage { get; set; }
        

        


        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        [NotMapped]
        public List<IFormFile>?ImageFiles { get; set; }
    }
}
