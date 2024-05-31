using Microsoft.EntityFrameworkCore;
using PetSpa.Data;
using PetSpa.Models.Domain;

namespace PetSpa.Repositories.BookingRepository
{
    public class SQLBookingRepository : IBookingRepository
    {
        private readonly PetSpaContext dbContext;

        public SQLBookingRepository(PetSpaContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public  async Task<Booking> CreateAsync(Booking booking)
        {
            await dbContext.Bookings.AddAsync(booking);
            await dbContext.SaveChangesAsync();
            return booking;
        }

        public async Task<List<Booking>> GetAllAsync()
        {
            return await dbContext.Bookings.Include("BookingDetails").Include("Cus").Include("Invoice").Include("Staff").Include("Voucher").ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(Guid BookingId)
        {
            return await dbContext.Bookings.Include("BookingDetails").Include("Cus").Include("Invoice").Include("Staff").Include("Voucher").FirstOrDefaultAsync(x => x.BookingId == BookingId);
        }

        public async Task<Booking?> UpdateAsync(Guid BookingId, Booking booking)
        {
            var existBoooking = await dbContext.Bookings.FirstOrDefaultAsync(x => x.BookingId == BookingId);
            if (existBoooking != null) return null;

            existBoooking.CusId = booking.CusId;
            existBoooking.StaffId = booking.StaffId;
            existBoooking.Status = booking.Status;
            existBoooking.StartDate = booking.StartDate;
            existBoooking.EndDate = booking.EndDate;
            existBoooking.BookingSchedule = booking.BookingSchedule;
            existBoooking.TotalAmount = booking.TotalAmount;
            existBoooking.Feedback = booking.Feedback;

            await dbContext.SaveChangesAsync();
            return existBoooking;
        }
    }
}
