using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlgebraWebShop2026.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        public string? Message { get; set; }

        [Required]
        public int OrderNumber { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}",ApplyFormatInEditMode=true)]
        public DateTime Created {  get; set; }

        [Required(ErrorMessage ="Total price is required!")]
        [Column(TypeName ="decimal(9,2)")]
        public decimal Total { get; set; }

        #region Billing info

        [Required(ErrorMessage ="First name is required!")]
        [StringLength(100)]
        public string BillingFirstname { get; set; }

        [Required(ErrorMessage = "Last name is required!")]
        [StringLength(200)]
        public string BillingLastname { get; set; }

        [Required]
        [StringLength(150)]
        [DataType(DataType.EmailAddress,ErrorMessage = "E-mail is not valid!")]

        public string BillingEmail { get; set; }

        [Required(ErrorMessage = "Phone number is required!")]
        [StringLength(100)]
        public string BillingPhone { get; set; }

        [Required(ErrorMessage = "Address is required!")]
        [StringLength(100)]
        public string BillingAddress { get; set; }

        [Required(ErrorMessage = "City is required!")]
        [StringLength(100)]
        public string BillingCity { get; set; }

        [Required(ErrorMessage = "ZIP is required!")]
        [StringLength(10)]
        public string BillingZIP { get; set; }

        [Required(ErrorMessage = "Country is required!")]
        [StringLength(100)]
        public string BillingCountry { get; set; }

        #endregion

        #region Shipping info

        [Required(ErrorMessage = "First name is required!")]
        [StringLength(100)]
        public string ShippingFirstname { get; set; }

        [Required(ErrorMessage = "Last name is required!")]
        [StringLength(200)]
        public string ShippingLastname { get; set; }

        [Required]
        [StringLength(150)]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail is not valid!")]
        public string ShippingEmail { get; set; }

        [Required(ErrorMessage = "Phone number is required!")]
        [StringLength(100)]
        public string ShippingPhone { get; set; }

        [Required(ErrorMessage = "Address is required!")]
        [StringLength(100)]
        public string ShippingAddress { get; set; }

        [Required(ErrorMessage = "City is required!")]
        [StringLength(100)]
        public string ShippingCity { get; set; }

        [Required(ErrorMessage = "ZIP is required!")]
        [StringLength(10)]
        public string ShippingZIP { get; set; }

        [Required(ErrorMessage = "Country is required!")]
        [StringLength(100)]
        public string ShippingCountry { get; set; }

        #endregion

        [ForeignKey("OrderId")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
