using PetSpa.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Hangfire;


namespace PetSpa.Repositories.BookingRepository
{
    public class BookingStatusChecker
    {
        private readonly PetSpaContext _context;

        public BookingStatusChecker(PetSpaContext context)
        {
            _context = context;
        }

        public async Task CheckAndUpdateBookingStatus()
        {
            var bookings = await _context.Bookings
                .Include(b => b.BookingDetails)
                .Where(b => b.Status == false && b.EndDate <= DateTime.Now)
                .ToListAsync();

            foreach (var booking in bookings)
            {
                booking.Status = true;
                foreach (var detail in booking.BookingDetails)
                {
                    detail.Status = true;
                }
            }

            await _context.SaveChangesAsync();
        }

        public static void ConfigureHangfireJobs(IServiceProvider serviceProvider)
        {
            var checker = serviceProvider.GetRequiredService<BookingStatusChecker>();
            RecurringJob.AddOrUpdate(
                "check-booking-status",
                () => checker.CheckAndUpdateBookingStatus(),
                Cron.MinuteInterval(5));
        }
    }

}
