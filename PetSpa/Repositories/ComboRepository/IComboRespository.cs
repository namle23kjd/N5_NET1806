using PetSpa.Models.Domain;

namespace PetSpa.Repositories.ComboRepository
{
    public interface IComboRespository
    {
        Task<Combo> CreateAsync(Combo combo);
        Task<List<Combo>> GetAllAsync();
        Task<Combo?> GetByIdAsync(Guid ComboId);
        Task<Combo?> UpdateAsync(Guid ComboID, Combo combo);
    }
}
