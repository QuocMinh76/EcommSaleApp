using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EcommSale.Models
{
    public class Product
    {
        [Required]
        public int ProductID { get; set; }

        [Required]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public string? Image { get; set; }

        [Display(Name = "Product Color")]
        public string? ProductColor { get; set; }

        [Required]
        [Display(Name = "Product Quantity")]
        public int? ProductCount { get; set; } = 0;

        [Required]
        [Display(Name = "Available")]
        public bool IsAvailable { get; set; }

        [Display(Name = "Category")]
        [Required]
        public int CategoryID { get; set; }
        [ForeignKey("CategoryID")]
        public Category Category { get; set; }

        [Display(Name = "Brand")]
        [Required]
        public int BrandID { get; set; }
        [ForeignKey("BrandID")]
        public Brand Brand { get; set; }
    }
}
