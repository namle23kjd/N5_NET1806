using PetSpa.Models.Domain;

namespace PetSpa.Repositories
{
    public interface IAccountRepository
    {
        Task<List<Account>> GetALLAsync();
        Task<Account> CreateAsync(Account account);
        Task<Account?> GetByIDAsybc(Guid AccId);
    }
}
