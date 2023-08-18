using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace Loza.Entities
{
    public class ReturnOrder
    {
        [Key]
        public int Request_Id { get; set; }
        public bool Confirmed { get; set; } = false;
        public bool isRejected { get; set; } = false;
        public string Reason { get; set; }
        [ForeignKey("Orders")]
        public int Order_Id { get; set; }


        public virtual Order Orders { get; set; }
        

    }
}
