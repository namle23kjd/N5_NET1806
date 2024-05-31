using PetSpa.Models.Domain;

namespace PetSpa.Repositories.AccountRepository
{
    public interface IAccountRepository
    {
        Task<List<Account>> GetALLAsync();
        Task<Account> CreateAsync(Account account);
        Task<Account> GetByIDAsybc(Guid AccId);
        Task<Account> UpdateAsync(Guid AccId, Account account);
        Task<Account> DeleteAsync(Guid AccId);
        Task AddStaffAsync(Guid accId);
        Task AddCustomerAsync(Guid accId);
        Task AddManagerAsync(Guid accId);
    }
}
