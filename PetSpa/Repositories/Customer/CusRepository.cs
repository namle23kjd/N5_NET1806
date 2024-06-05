
using Microsoft.EntityFrameworkCore;
using PetSpa.Data;
using PetSpa.Models.Domain;

namespace PetSpa.Repositories.Customer
{
    public class CusRepository : ICusRepository
    {

        private readonly PetSpaContext _context;
        public CusRepository(PetSpaContext context)
        {
            _context = context;

        }

        public async Task<Models.Domain.Customer> DeleteAsync(Guid Id)
        {
            var cusModel = await _context.Customers.FirstOrDefaultAsync(x => x.CusId == Id);
            if (cusModel == null)
            {
                return null;
            }
            _context.Customers.Remove(cusModel);
            await _context.SaveChangesAsync();
            return cusModel;
        }

        public async Task<List<Models.Domain.Customer>> GetALLAsync()
        {
            return await _context.Customers.Include(p => p.Pets).ToListAsync();
        }

        public async  Task<Models.Domain.Customer?> getByIdAsync(Guid id)
        {
            return await _context.Customers.Include(p => p.Pets).FirstOrDefaultAsync(r => r.CusId == id);
        }


        public async Task<bool> StockExit(Guid id)
        {
            return await _context.Customers.AnyAsync(s => s.CusId == id);
        }

        public async Task<Models.Domain.Customer> UpdateAsync(Guid id, Models.Domain.Customer customer)
        {
            var existingCus = await _context.Customers.FirstOrDefaultAsync(x => x.CusId == id);

            if (existingCus == null)
            {
                return null;
            }

            existingCus.PhoneNumber = customer.PhoneNumber;
            existingCus.FullName = customer.FullName;
            existingCus.Gender = customer.Gender;
            
           

            await _context.SaveChangesAsync();
            return existingCus;
        }

    }
}
