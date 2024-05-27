

using PetSpa.Models.Domain;

namespace PetSpa.Repositories
{
    public interface IPetRepository
    {
        Task<List<PetSpa.Models.Domain.Pet>> GetALLAsync();
        Task<PetSpa.Models.Domain.Pet?> getByIdAsync(Guid id);
        Task<PetSpa.Models.Domain.Pet> CreateAsync(PetSpa.Models.Domain.Pet pet);
        //Task<Account> GetByIDAsybc(Guid AccId);
        //Task<Account> UpdateAsync(Guid AccId, Account account);
        Task<PetSpa.Models.Domain.Pet> DeleteAsync(Guid Id);
        Task<PetSpa.Models.Domain.Pet> UpdateAsync(Guid id, PetSpa.Models.Domain.Pet pet);
    }
}
