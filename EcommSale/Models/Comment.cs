using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EcommSale.Models
{
    public class Comment
    {
        [Key]
        public int CommentID { get; set; }

        [Required]
        public string CommenterID { get; set; }

        [Required]
        [StringLength(100)]
        public string CommenterName { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        [StringLength(500)]
        public string Content { get; set; }

        [Required]
        public DateTime PostedDate { get; set; }

        [ForeignKey("CommenterID")]
        public ApplicationUser Commenter { get; set; }

        [ForeignKey("ProductID")]
        public Product Product { get; set; }
    }
}
