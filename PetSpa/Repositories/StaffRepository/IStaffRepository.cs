using PetSpa.Models.Domain;

namespace PetSpa.Repositories.StaffRepository
{
    public interface IStaffRepository
    {
        Task<Staff> CreateAsync(Staff staff);
        Task<List<Staff>> GetALlAsync();
        Task<Staff?> GetByIdAsync(Guid StaffId);
        Task<Staff?> UpdateAsync(Guid StaffId, Staff staff);
    }
}
