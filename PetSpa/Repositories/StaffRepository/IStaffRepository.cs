using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Booking;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetSpa.Repositories.StaffRepository
{
    public interface IStaffRepository
    {
        Task<List<Staff>> GetALlAsync();
        Task<Staff?> GetByIdAsync(Guid StaffId);
        Task<Staff?> UpdateAsync(Guid StaffId, Staff staff);
        Task<Staff> CreateAsync(Staff staff); // Thêm hàm CreateAsync
        Task<List<Booking>> GetBookingsByStatusAsync(Guid staffId, BookingStatus status);
    }
}
