
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Loza.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }


        [StringLength(100)]
        public string Name { get; set; }


        [StringLength(1000)]
        public string Description { get; set; }


        [Range(0, 10000)]
        public decimal Price { get; set; } = 0;


        [Range(0, 7)]
        public int Category { get; set; }


        [StringLength(24)]
        public string Color { get; set; }

        public int ColorNo { get; set; }


        [Range(0, 100)]
        public decimal Quantity { get; set; } = 0;

        

        public string? ProductImage { get; set; }

        public string? ProductDimensions { get; set; }



        /* [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{dd-MM-yyyy}")]
          public DateTime CrateDateTime { get; set; }
         [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{dd-MM-yyyy}")]
          public DateTime UpdateDateTime { get; set;}*/


        public ICollection<Photo>? Photos { get; set; }

        //  [NotMapped]
        // public IFormFile? ImageFile { get; set; }
        //   [NotMapped]
        //  public List<IFormFile>? FormFiles { get; set; }




    }
}
