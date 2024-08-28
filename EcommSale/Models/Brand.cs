using System.ComponentModel.DataAnnotations;

namespace ECommSale.Models
{
    public class Brand
    {
        [Required]
        public int BrandID { get; set; }
        [Required]
        [Display(Name = "Brand")]
        public string BrandName { get; set; }
    }
}
