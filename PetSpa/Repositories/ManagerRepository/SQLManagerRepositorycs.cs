using Microsoft.EntityFrameworkCore;
using PetSpa.Data;
using PetSpa.Models.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetSpa.Repositories.ManagerRepository
{
    public class SQLManagerRepositorycs : IManagerRepository
    {
        private readonly PetSpaContext _dbContext;

        public SQLManagerRepositorycs(PetSpaContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<List<Manager>> GetAllAsync()
        {
            return await _dbContext.Managers.ToListAsync();
        }

        public async Task<Manager?> GetByIDAsync(Guid ManaId)
        {
            return await _dbContext.Managers
                .Include(m => m.Staffs)
                .Include(m => m.Bookings)
                .FirstOrDefaultAsync(x => x.ManaId == ManaId);
        }

        public async Task<Manager?> UpdateAsync(Guid ManaId, Manager manager)
        {
            var existingManager = await _dbContext.Managers.FirstOrDefaultAsync(x => x.ManaId == ManaId);
            if (existingManager == null)
            {
                return null;
            }
            existingManager.FullName = manager.FullName;
            existingManager.PhoneNumber = manager.PhoneNumber;
            existingManager.Gender = manager.Gender;
            await _dbContext.SaveChangesAsync();
            return existingManager;
        }

        public async Task<Manager> CreateAsync(Manager manager)
        {
            await _dbContext.Managers.AddAsync(manager);
            await _dbContext.SaveChangesAsync();
            return manager;
        }
    }
}
