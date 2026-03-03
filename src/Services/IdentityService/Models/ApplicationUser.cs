using Microsoft.AspNetCore.Identity;

namespace IdentityService.Models;

// IdentityUser sınıfından miras alıyoruz.
// Bu sayede password hashing, email, phone number gibi özellikler bedavadan geliyor.
public class ApplicationUser : IdentityUser
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
}