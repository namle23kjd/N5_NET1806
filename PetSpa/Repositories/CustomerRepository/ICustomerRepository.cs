using PetSpa.Models.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetSpa.Repositories.CustomerRepository
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(Guid CusId);
        Task<Customer?> UpdateAsync(Guid CusId, Customer customer);
        Task<Customer?> GetByIdBookingAsync(Guid CusId);
        Task<bool> IsUniquePhoneNumberAsync(Guid id, string newPhoneNumber);
        Task<bool> DeleteAsync(Guid CusId);
       
    }
}
