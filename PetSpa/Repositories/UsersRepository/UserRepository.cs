using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetSpa.Models.Domain;

namespace PetSpa.Repositories.UsersRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            this._userManager = userManager;
        }
        public async Task<List<ApplicationUser>> GetAllUserAsync()
        {
            return await _userManager.Users.ToListAsync();
        }
    }
}
