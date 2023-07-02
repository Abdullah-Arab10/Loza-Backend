using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Loza.Entities;
using Loza.Models.User;

namespace Loza.Entities
{
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; }

        public string ProductName { get; set; }
        public int Quant { get; set; } = 1;

        public decimal price { get; set; }


        public decimal Total
        {
            get { return price * Quant; }
        }


        public int UserId{ get; set; }



        public int? ProductId { get; set; }
        public Product Product { get; set;}



     
        
     /*   public ShoppingCart(Product pro) 
        {
            ProductName = pro.Name;
            price = pro.Price; 
        
        }*/


    }
}
