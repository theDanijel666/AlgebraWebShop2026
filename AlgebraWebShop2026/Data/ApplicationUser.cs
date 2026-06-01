using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AlgebraWebShop2026.Data;
using AlgebraWebShop2026.Models;
// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [StringLength(100)]
    public string? Ime { get; set; }

    [StringLength(200)]
    public string? Prezime { get; set; }

    [StringLength (200)]
    public string? Adresa { get; set;  }

    [StringLength(100)]
    public string? Grad { get; set; }

    [StringLength(10)]
    public string? PB { get; set; }

    [StringLength(100)]
    public string? Drzava { get; set; }

    [ForeignKey("UserId")]
    public virtual ICollection<Order> Orders { get; set; }

}
