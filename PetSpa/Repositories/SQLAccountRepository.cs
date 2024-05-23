using Microsoft.EntityFrameworkCore;
using PetSpa.Data;
using PetSpa.Models.Domain;

namespace PetSpa.Repositories
{
    public class SQLAccountRepository : IAccountRepository
    {
        private readonly PetSpaContext dbContext;

        public SQLAccountRepository(PetSpaContext dbContext) {
            this.dbContext = dbContext;
        }

        public async Task<Account> CreateAsync(Account account)
        {
            await dbContext.Accounts.AddAsync(account);
            await dbContext.SaveChangesAsync();
            return account;
        }

        public async Task<List<Account>> GetALLAsync()
        {
            return await dbContext.Accounts.ToListAsync();
        }

        public async Task<Account?> GetByIDAsybc(Guid AccId)
        {
            return await dbContext.Accounts.FirstOrDefaultAsync(x => x.AccId == AccId);
        }

        public async Task<Account> UpdateAsync(Guid AccId, Account account)
        {
            var existingAccount = await dbContext.Accounts.FirstOrDefaultAsync( x => x.AccId == AccId);

            if (existingAccount == null)
            {
                return null;
            }
            existingAccount.UserName = account.UserName;
            existingAccount.PassWord= account.PassWord;
            existingAccount.Role = account.Role;

            await dbContext.SaveChangesAsync();
            return existingAccount;
        }

        public async Task<Account> DeleteAsync(Guid AccId)
        {
            var existingAccount = await dbContext.Accounts.FirstOrDefaultAsync(x => x.AccId == AccId);
            if (existingAccount == null)
            {
                return null;
            }
            dbContext.Accounts.Remove(existingAccount);
            await dbContext.SaveChangesAsync();
            return existingAccount;
        }
    }
}
