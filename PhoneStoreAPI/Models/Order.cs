using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PhoneStoreAPI.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("User")]
        public Guid CustomerId { get; set; }

        public virtual User Customer { get; set; }

        public DateTime? OrderDate { get; set; }

        [StringLength(60)]
        public string? Address { get; set; }

        public double Amount { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}