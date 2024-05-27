using Microsoft.EntityFrameworkCore;
using PetSpa.Data;
using PetSpa.Models.Domain;

namespace PetSpa.Repositories
{
    public class SQLManagerRepositorycs : IManagerRepository
    {
        private readonly PetSpaContext dbContext;

        public SQLManagerRepositorycs(PetSpaContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Manager> CreateAsync(Manager manager)
        {
            await dbContext.Managers.AddAsync(manager);
            await dbContext.SaveChangesAsync();
            return manager;
        }

        public async Task<List<Manager>> GetAllAsync()
        {
            return await dbContext.Managers.Include("Acc").Include("Jobs").Include("Vouchers").ToListAsync();
        }

        public async Task<Manager?> GetByIDAsync(int ManaId)
        {
            return await dbContext.Managers.Include("Acc").Include("Jobs").Include("Vouchers").FirstOrDefaultAsync(x => x.ManaId == ManaId);
        }

        public async Task<Manager?> UpdateAsync(int ManaId, Manager manager)
        {
            var exsitingManager = await dbContext.Managers.FirstOrDefaultAsync(x => x.ManaId == ManaId);
            if ( exsitingManager == null)
            {
                return null;
            }
            exsitingManager.FullName  = manager.FullName;
            exsitingManager.PhoneNumber = manager.PhoneNumber;
            exsitingManager.Gender = manager.Gender;
            exsitingManager.Acc = manager.Acc;
            exsitingManager.Jobs = manager.Jobs;
            exsitingManager.Vouchers = manager.Vouchers;

            await dbContext.SaveChangesAsync();
            return exsitingManager;
        }
    }
}
