using Microsoft.AspNetCore.Identity;

namespace PetSpa.Models.Domain
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? refreshToken { get; set; }

        public DateTime refreshTokenExpiry { get; set; }          

        
    }
}
