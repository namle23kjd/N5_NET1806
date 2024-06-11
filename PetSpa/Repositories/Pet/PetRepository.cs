
using Microsoft.EntityFrameworkCore;
using PetSpa.Data;
using PetSpa.Models.Domain;

namespace PetSpa.Repositories.Pet
{
    public class PetRepository : IPetRepository
    {

        private readonly PetSpaContext _context;
        public PetRepository(PetSpaContext context)
        {
            _context = context;

        }

        public async Task<Models.Domain.Pet> CreateAsync(Models.Domain.Pet pet)
        {
            await _context.Pets.AddAsync(pet);
            await _context.SaveChangesAsync();
            return pet;
        }

        public async Task<Models.Domain.Pet> DeleteAsync(Guid Id)
        {
            var commentModel = await _context.Pets.FirstOrDefaultAsync(x => x.PetId == Id);
            if (commentModel == null)
            {
                return null;
            }
            _context.Pets.Remove(commentModel);
            await _context.SaveChangesAsync();
            return commentModel;
        }

        public async Task<List<Models.Domain.Pet>> GetALLAsync()
        {
            return await _context.Pets.ToListAsync();
        }

        public async Task<Models.Domain.Pet?> getByIdAsync(Guid id)
        {
            return await _context.Pets.FirstOrDefaultAsync(r => r.PetId == id);
        }

        public async Task<Models.Domain.Pet> UpdateAsync(Guid id, Models.Domain.Pet pet)
        {
            var existingPet = await _context.Pets.FirstOrDefaultAsync(x => x.PetId == id);

            if (existingPet == null)
            {
                return null;
            }

             existingPet.PetBirthday = pet.PetBirthday;
             if(existingPet.Status != pet.Status) existingPet.Status = pet.Status;
            existingPet.PetWeight = pet.PetWeight;
            existingPet.PetHeight = pet.PetHeight;
            existingPet.Image = pet.Image;
            existingPet.PetName = pet.PetName;
            existingPet.PetType = pet.PetType;

            await _context.SaveChangesAsync();
            return existingPet;
            
        }
    }
}
