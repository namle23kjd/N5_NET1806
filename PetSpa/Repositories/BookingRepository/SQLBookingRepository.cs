using Microsoft.EntityFrameworkCore;
using PetSpa.Data;
using PetSpa.Models.Domain;
using System;
using System.Collections.Generic;
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

            await dbContext.SaveChangesAsync();
            return existingBooking;
        }
    }
}
