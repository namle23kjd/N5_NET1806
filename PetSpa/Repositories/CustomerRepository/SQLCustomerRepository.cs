using Microsoft.EntityFrameworkCore;
using PetSpa.Data;
using PetSpa.Models.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace PetSpa.Repositories.CustomerRepository
{
    public class SQLCustomerRepository : ICustomerRepository

    {
        private readonly PetSpaContext _dbContext;

        public SQLCustomerRepository(PetSpaContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<bool> DeleteAsync(Guid CusId)
        {
            var customer = await _dbContext.Customers.Include(c => c.User).FirstOrDefaultAsync(x => x.CusId == CusId);
            if( customer == null) return false;
            customer.User.Status = false;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            return await _dbContext.Customers
                .Include(c => c.User)
                .Include(c => c.Pets)
                .Where(c => c.User.Status == true) // Lọc những khách hàng có Status là true
                .ToListAsync();
        }

        public async Task<Customer?> GetByIdAsync(Guid CusId)
        {
            return await _dbContext.Customers
                .Include(c => c.User)
                .Include(c => c.Pets)
                .FirstOrDefaultAsync(x => x.CusId == CusId);
        }

        public async Task<Customer?> UpdateAsync(Guid CusId, Customer customer)
        {
            var existingCustomer = await _dbContext.Customers.FirstOrDefaultAsync(x => x.CusId == CusId);
            if (existingCustomer == null)
            {
                return null;
            }
            existingCustomer.FullName = customer.FullName;
            existingCustomer.Gender = customer.Gender;
            existingCustomer.PhoneNumber = customer.PhoneNumber;
            existingCustomer.CusRank = customer.CusRank;
            await _dbContext.SaveChangesAsync();
            return existingCustomer;
        }

        public async Task<Customer?> GetByIdBookingAsync(Guid CusId)
        {
            return await _dbContext.Customers
                .Include(c => c.Bookings).ThenInclude(bd => bd.BookingDetails).ThenInclude(se => se.Service)
                .FirstOrDefaultAsync(x => x.CusId == CusId);
        }

        public async Task<Customer?> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _dbContext.Customers.FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber);
        }
    }
}
