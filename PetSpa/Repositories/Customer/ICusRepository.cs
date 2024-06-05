using Microsoft.EntityFrameworkCore;

namespace PetSpa.Repositories.Customer
{
    public interface ICusRepository
    {
        Task<List<PetSpa.Models.Domain.Customer>> GetALLAsync();
        Task<PetSpa.Models.Domain.Customer?> getByIdAsync(Guid id);
         Task<bool> StockExit(Guid id);
        Task<PetSpa.Models.Domain.Customer> DeleteAsync(Guid Id);
        Task<PetSpa.Models.Domain.Customer> UpdateAsync(Guid id, PetSpa.Models.Domain.Customer pet);
        Task<PetSpa.Models.Domain.Customer> CreateAsync(PetSpa.Models.Domain.Customer customer);
    }
}
