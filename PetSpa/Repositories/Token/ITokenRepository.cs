using Microsoft.AspNetCore.Identity;
using PetSpa.Models.Domain;
namespace PetSpa.Repositories.Token
{
    public interface ITokenRepository
    {
        string CreateJWTToken(ApplicationUser user, string role, int minute);
    }
}
