using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PhoneStoreAPI.Models
{
    public class Products
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public double UnitPrice { get; set; }

        [StringLength(50)]
        public string? Image { get; set; }

        public DateTime? AvailableDate { get; set; }

        public bool Available { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

        public int Quantity { get; set; }

        public string? Description { get; set; }

        public double ViewCount { get; set; }

        public bool Special { get; set; }
    }
}