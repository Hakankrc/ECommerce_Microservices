using Microsoft.AspNetCore.Identity;

namespace IdentityService.Models;

public class ApplicationUser : IdentityUser
{
    
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;

    
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}