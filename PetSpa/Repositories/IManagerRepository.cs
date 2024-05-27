using PetSpa.Models.Domain;

namespace PetSpa.Repositories
{
    public interface IManagerRepository
    {
        Task<Manager> CreateAsync (Manager manager);
        Task<List<Manager>> GetAllAsync();
        Task<Manager> GetByIDAsync(int ManaId);
        Task<Manager?> UpdateAsync(int ManaId, Manager manager);
    }
}
