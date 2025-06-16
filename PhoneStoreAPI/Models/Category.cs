using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PhoneStoreAPI.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public string? NameVN { get; set; }

        
    }
}