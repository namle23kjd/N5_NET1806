using Microsoft.AspNetCore.Identity;

namespace PetSpa.Repositories.Token
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, string role, int minute);
    }
}
