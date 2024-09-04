using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EcommSale.Models
{
    public class Category
    {
        [Required]
        public int CategoryID { get; set; }
        [Required]
        [Display(Name = "Category")]
        public string CategoryName { get; set; }
    }
}
