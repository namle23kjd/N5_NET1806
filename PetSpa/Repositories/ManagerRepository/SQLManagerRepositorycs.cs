using Microsoft.EntityFrameworkCore;
using PetSpa.Data;
using PetSpa.Models.Domain;

namespace PetSpa.Repositories.ManagerRepository
{
    public class SQLManagerRepositorycs : IManagerRepository
    {
        private readonly PetSpaContext dbContext;

        public SQLManagerRepositorycs(PetSpaContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Manager>> GetAllAsync()
        {
            return await dbContext.Managers.ToListAsync();
        }

        public async Task<Manager?> GetByIDAsync(Guid ManaId)
        {
            return await dbContext.Managers.FirstOrDefaultAsync(x => x.ManaId == ManaId);
        }

        public async Task<Manager?> UpdateAsync(Guid ManaId, Manager manager)
        {
            var exsitingManager = await dbContext.Managers.FirstOrDefaultAsync(x => x.ManaId == ManaId);
            if (exsitingManager == null)
            {
                return null;
            }
            exsitingManager.FullName = manager.FullName;
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
