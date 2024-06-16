using Microsoft.EntityFrameworkCore;
using PetSpa.Data;
using PetSpa.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetSpa.Repositories.BookingRepository
{
    public class SQLBookingRepository : IBookingRepository
    {
        private readonly PetSpaContext dbContext;

        public SQLBookingRepository(PetSpaContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Booking> CreateAsync(Booking booking)
        {
            await dbContext.Bookings.AddAsync(booking);
            await dbContext.SaveChangesAsync();
            return booking;
        }

        public async Task<List<Booking>> GetAllAsync()
        {
            return await dbContext.Bookings
                .Include(b => b.BookingDetails)
                .ThenInclude(bd => bd.Combo)
                .Include(b => b.BookingDetails)
                .ThenInclude(bd => bd.Service)
                .Include(b => b.Customer)
                .Include(b => b.Manager)
                .Include(b => b.Invoice)
                .Include(b => b.Voucher)
                .ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(Guid BookingId)
        {
            return await dbContext.Bookings
                .Include(b => b.BookingDetails)
                .ThenInclude(bd => bd.Combo)
                .Include(b => b.BookingDetails)
                .ThenInclude(bd => bd.Service)
                .Include(b => b.Customer)
                .Include(b => b.Manager)
                .Include(b => b.Invoice)
                .Include(b => b.Voucher)
                .FirstOrDefaultAsync(b => b.BookingId == BookingId);
        }

        public async Task<Booking?> UpdateAsync(Guid BookingId, Booking booking)
        {
            var existingBooking = await dbContext.Bookings.FirstOrDefaultAsync(b => b.BookingId == BookingId);
            if (existingBooking == null) return null;

            existingBooking.CusId = booking.CusId;
            existingBooking.Status = booking.Status;
            existingBooking.BookingSchedule = booking.BookingSchedule;
            existingBooking.TotalAmount = booking.TotalAmount;
            existingBooking.Feedback = booking.Feedback;
            existingBooking.StartDate = booking.StartDate;
            existingBooking.EndDate = booking.EndDate;
            existingBooking.CheckAccept = booking.CheckAccept;

            await dbContext.SaveChangesAsync();
            return existingBooking;
        }


        public async Task<bool> IsScheduleTakenAsync(DateTime bookingSchedule)
        {
            return await dbContext.Bookings.AnyAsync(b => b.BookingSchedule == bookingSchedule && b.Status);

        }

        public async Task<Manager> GetManagerWithLeastBookingsAsync()
        {
            return await dbContext.Managers
                .OrderBy(m => dbContext.Bookings.Count(b => b.ManaId == m.ManaId))
                .FirstOrDefaultAsync();
        }

        public async Task<List<Booking>> GetCompletedBookingsAsync()
        {
            return await dbContext.Bookings
                .Include(b => b.BookingDetails)
                .Where(b => b.Status == true)
                .ToListAsync();
        }

        public List<Staff> GetAvailableStaffsForStartTime(DateTime startTime, DateTime endTime, Guid? staffId = null)
        {
            

            // If a specific staffId is provided, filter the query to check availability of that staff member
            if (staffId.HasValue)
            {
                // Check availability for the specific staff member
                var staff = dbContext.Staff
                    .Include(s => s.BookingDetails)
                    .ThenInclude(bd => bd.Booking)
                    .Where(s => s.StaffId == staffId.Value && !s.BookingDetails.Any(bd =>
                        (startTime < bd.Booking.EndDate && endTime > bd.Booking.StartDate) || // Overlap check
                        (endTime > bd.Booking.StartDate && startTime < bd.Booking.EndDate)    // Overlap check
                    ))
                    .ToList();

                return staff;
            }

            return dbContext.Staff
            .Include(s => s.BookingDetails)
            .ThenInclude(bd => bd.Booking)
            .Where(s => !s.BookingDetails.Any(bd =>
                (startTime < bd.Booking.EndDate && endTime > bd.Booking.StartDate) || // Overlap check
                (endTime > bd.Booking.StartDate && startTime < bd.Booking.EndDate)    // Overlap check
            ))
            .ToList();
        }
      
    }
}
