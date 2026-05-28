using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace AlgebraWebShop2026.Data;
// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [StringLength(100)]
    public string? Ime { get; set; }

    [StringLength(200)]
    public string? Prezime { get; set; }

    [StringLength (200)]
    public string? Adresa { get; set;  }

}
