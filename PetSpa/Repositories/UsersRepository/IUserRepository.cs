using PetSpa.Models.Domain;
using PetSpa.Models.DTO.UserDTO;

namespace PetSpa.Repositories.UsersRepository
{
    public interface IUserRepository
    {
        Task<List<UserDTO>> GetAllUserAsync();
    }
}
