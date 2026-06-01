using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlgebraWebShop2026.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100,MinimumLength =2)]
        public string Title { get; set; }


        public int? ParentCathegoryId { get; set; }

        [ForeignKey(nameof(ParentCathegoryId))]
        public Category ParentCategory { get; set; }

        [ForeignKey("CategoryId")]
        public virtual ICollection<ProductCategory> ProductCategories { get; set; }
    }
}
