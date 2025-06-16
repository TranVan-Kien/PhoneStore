using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PhoneStoreAPI.Models
{
    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }

        public virtual Order Order { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public virtual Products Product { get; set; }

        public double UnitPrice { get; set; }

        public int Quantity { get; set; }

        public double Discount { get; set; }
    }
}