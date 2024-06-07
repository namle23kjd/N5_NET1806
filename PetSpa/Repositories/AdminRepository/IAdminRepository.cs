using PetSpa.Models.Domain;

namespace PetSpa.Repositories.AdminRepository
{
    public interface IAdminRepository
    {
        Task<List<Admin>> GetAll();
        Task<Admin> CreateAsync(Admin admin);
    }
}
