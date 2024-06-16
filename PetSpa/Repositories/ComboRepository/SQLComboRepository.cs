using Microsoft.EntityFrameworkCore;
using PetSpa.Data;
using PetSpa.Models.Domain;

namespace PetSpa.Repositories.ComboRepository
{
    public class SQLComboRepository : IComboRespository
    {
        private readonly PetSpaContext dbContext;

        public SQLComboRepository(PetSpaContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Combo> CreateAsync(Combo combo)
        {
            await dbContext.Combos.AddAsync(combo);
            await dbContext.SaveChangesAsync();
            return combo;
        }

        public async Task<List<Combo>> GetAllAsync()
        {
            return await dbContext.Combos
                .Include(c => c.Services)
                .Where(c => c.Status == true) // Only get combos with Status true
                .ToListAsync();
        }

        public async Task<Combo?> GetByIdAsync(Guid ComboId)
        {
            return await dbContext.Combos
                .Include(c => c.BookingDetails)
                .Include(c => c.Services)
                .FirstOrDefaultAsync(x => x.ComboId == ComboId && x.Status == true); // Only get if Status true
        }

        public async Task<Combo?> UpdateAsync(Guid ComboID, Combo combo)
        {
            var existingCombo = await dbContext.Combos.FirstOrDefaultAsync(x => x.ComboId == ComboID);
            if (existingCombo == null) return null;

            existingCombo.ComboType = combo.ComboType;
            existingCombo.Price = combo.Price;
            existingCombo.Status = combo.Status;
            await dbContext.SaveChangesAsync();

            return existingCombo;
        }

        public async Task<Combo?> DeleteAsync(Guid ComboID)
        {
            var existingCombo = await dbContext.Combos.FirstOrDefaultAsync(x => x.ComboId == ComboID);
            if (existingCombo == null) return null;

            existingCombo.Status = false;
            await dbContext.SaveChangesAsync();

            return existingCombo;
        }
    }
}
