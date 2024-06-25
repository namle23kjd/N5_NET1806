using PetSpa.Models.Domain;

namespace PetSpa.Repositories.UsersRepository
{
    public interface IUserRepository
    {
        Task<List<ApplicationUser>> GetAllUserAsync();
    }
}
