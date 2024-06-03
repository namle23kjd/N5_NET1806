using Microsoft.AspNetCore.Identity;

namespace PetSpa.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string refreshToken { get; set; }

        public DateTime refreshTokenExpiry { get; set; }          

        
    }
}
