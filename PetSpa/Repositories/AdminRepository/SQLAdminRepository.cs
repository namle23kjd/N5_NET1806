using Microsoft.EntityFrameworkCore;
using PetSpa.Data;
using PetSpa.Models.Domain;

namespace PetSpa.Repositories.AdminRepository
{
    public class SQLAdminRepository : IAdminRepository
    {
        private readonly PetSpaContext dbContext;

        public SQLAdminRepository(PetSpaContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<List<Admin>> GetAll()
        {
           return await dbContext.Admins.ToListAsync();
        }
    }
}
