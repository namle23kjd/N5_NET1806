using Microsoft.EntityFrameworkCore;
using PetSpa.Data;
using PetSpa.Models.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetSpa.Repositories
{
    public class PetRepository : IPetRepository
    {
        private readonly PetSpaContext _context;

        public PetRepository(PetSpaContext context)
        {
            _context = context;
        }

        public async Task<Pet> CreateAsync(Pet pet)
        {
            await _context.Pets.AddAsync(pet);
            await _context.SaveChangesAsync();
            return pet;
        }

        public async Task<Pet?> DeleteAsync(Guid Id)
        {
            var pet = await _context.Pets.FirstOrDefaultAsync(x => x.PetId == Id);
            if (pet == null)
            {
                return null;
            }

            // Thay đổi trạng thái từ true thành false
            pet.Status = false;
            await _context.SaveChangesAsync();
            return pet;
        }

        public async Task<List<Pet>> GetALLAsync()
        {
            return await _context.Pets.ToListAsync();
        }

        public async Task<Pet?> GetByIdAsync(Guid id)
        {
            return await _context.Pets.FirstOrDefaultAsync(r => r.PetId == id);
        }

        public async Task<Pet?> UpdateAsync(Guid id, Pet pet)
        {
            var existingPet = await _context.Pets.FirstOrDefaultAsync(x => x.PetId == id);

            if (existingPet == null)
            {
                return null;
            }

            existingPet.PetBirthday = pet.PetBirthday;
            existingPet.Status = pet.Status;
            existingPet.PetWeight = pet.PetWeight;
            existingPet.Image = pet.Image;
            existingPet.PetName = pet.PetName;
            existingPet.PetType = pet.PetType;

            await _context.SaveChangesAsync();
            return existingPet;
        }
    }
}
