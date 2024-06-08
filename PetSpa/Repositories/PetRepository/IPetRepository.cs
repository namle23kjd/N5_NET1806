using PetSpa.Models.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetSpa.Repositories
{
    public interface IPetRepository
    {
        Task<List<Pet>> GetALLAsync();
        Task<Pet?> GetByIdAsync(Guid id);
        Task<Pet> CreateAsync(Pet pet);
        Task<Pet?> DeleteAsync(Guid Id);
        Task<Pet?> UpdateAsync(Guid id, Pet pet);
    }
}
