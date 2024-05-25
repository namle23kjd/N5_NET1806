using PetSpa.Models.Domain;

namespace PetSpa.Repositories
{
    public interface IStaffRepository
    {
        Task<Staff> CreateAsync(Staff staff);
        Task<List<Staff>> GetAllAsync();
        Task<Staff?> GetByIdAsync(Guid StaffId);
        Task<Staff?> UpdateAsync(Guid StaffId, Staff staff);
    }
}
