using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Admin;

namespace PetSpa.Repositories.AdminRepository
{
    public interface IAdminRepository
    {
        Task<List<Admin>> GetAllAsync();
        Task<bool> CreateAdminAsync(AddAdminRequestDTO adminRequest);
        Task<Admin> UpdateAsync(Guid AdminId, Admin admin);
        Task<Admin> FindAdminAsync(Guid AdminId);
    }
}
