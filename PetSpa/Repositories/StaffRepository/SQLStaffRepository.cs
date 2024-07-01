using Microsoft.EntityFrameworkCore;
using PetSpa.Data;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Booking;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetSpa.Repositories.StaffRepository
{
    public class SQLStaffRepository : IStaffRepository
    {
        private readonly PetSpaContext _dbContext;

        public SQLStaffRepository(PetSpaContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<List<Staff>> GetALlAsync()
        {
            return await _dbContext.Staff
                .Include(s => s.User)
                .Include(s => s.BookingDetails)
                .Include(s => s.Manager)
                .ToListAsync();
        }

        public async Task<Staff?> GetByIdAsync(Guid StaffID)
        {
            return await _dbContext.Staff
                .Include(s => s.User)
                .Include(s => s.BookingDetails)
                .Include(s => s.Manager)
                .FirstOrDefaultAsync(x => x.StaffId == StaffID);
        }

        public async Task<Staff?> UpdateAsync(Guid StaffId, Staff staff)
        {
            var existingStaff = await _dbContext.Staff.FirstOrDefaultAsync(x => x.StaffId == StaffId);
            if (existingStaff == null)
            {
                return null;
            }
            existingStaff.FullName = staff.FullName;
            existingStaff.Gender = staff.Gender;
            await _dbContext.SaveChangesAsync();
            return existingStaff;
        }

        public async Task<Staff> CreateAsync(Staff staff)
        {
            await _dbContext.Staff.AddAsync(staff);
            await _dbContext.SaveChangesAsync();
            return staff;
        }

        public async Task<List<Booking>> GetBookingsByStatusAsync(Guid staffId, BookingStatus status)
        {
            return await _dbContext.Bookings
        .Include(b => b.Customer)
        .Include(b => b.BookingDetails)
            .ThenInclude(bd => bd.Service)
        .Include(b => b.BookingDetails)
            .ThenInclude(bd => bd.Pet)
        .Where(b => b.BookingDetails.Any(bd => bd.StaffId == staffId) && b.Status == status)
        .ToListAsync();
        }
    }
}
