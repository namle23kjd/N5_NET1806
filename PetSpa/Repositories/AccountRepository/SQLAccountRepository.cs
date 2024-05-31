using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using PetSpa.Data;
using PetSpa.Models.Domain;

namespace PetSpa.Repositories.AccountRepository
{
    public class SQLAccountRepository : IAccountRepository
    {
        private readonly PetSpaContext dbContext;

        public SQLAccountRepository(PetSpaContext dbContext)
        {
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
            var existingAccount = await dbContext.Accounts.FirstOrDefaultAsync(x => x.AccId == AccId);

            if (existingAccount == null)
            {
                return null;
            }
            existingAccount.UserName = account.UserName;
            existingAccount.PassWord = account.PassWord;
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

        public async Task AddStaffAsync(Guid accountId)
        {
            var staff = new Staff
            {
                StaffId = Guid.NewGuid(),
                AccId = accountId,
                FullName = "", // Cập nhật giá trị mặc định hoặc từ DTO
                Gender = "" // Cập nhật giá trị mặc định hoặc từ DTO
            };
            await dbContext.Staff.AddAsync(staff);
            await dbContext.SaveChangesAsync();
        }


        public async Task AddCustomerAsync(Guid accountId)
        {
            var customer = new Customer
            {
                CusId = Guid.NewGuid(),
                AccId = accountId,
                FullName = "", // Cập nhật giá trị mặc định hoặc từ DTO
                Gender = "", // Cập nhật giá trị mặc định hoặc từ DTO
                PhoneNumber = "", // Cập nhật giá trị mặc định hoặc từ DTO
                CusRank = "" // Cập nhật giá trị mặc định hoặc từ DTO
            };
            await dbContext.Customers.AddAsync(customer);
            await dbContext.SaveChangesAsync();
        }

        public async Task AddManagerAsync(Guid accountID)
        {
            var manager = new Manager
            {
                ManaId = Guid.NewGuid(),
                AccId = accountID,
                FullName = "",
                Gender = "",
                PhoneNumber = ""
            };
            await dbContext.Managers.AddAsync(manager);
            await dbContext.SaveChangesAsync();

        }
    }
}
