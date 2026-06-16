using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlgebraWebShop2026.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        public int ProductId { get; set; }

        [Required]
        [Column(TypeName = "decimal(9,2)")]
        public decimal Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(9,2)")]
        public decimal Price { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string MesuringUnit { get; set; }

        [Column(TypeName = "decimal(9,2)")]
        public decimal Discount { get; set; } = 0;

        [NotMapped]
        public string ProductTitle { get; set; }

    }
}
