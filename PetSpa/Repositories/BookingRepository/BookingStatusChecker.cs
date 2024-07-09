using PetSpa.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using PetSpa.Models.DTO.Booking;


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
            var now = DateTime.Now;

            // Kiểm tra các booking chưa bắt đầu và thời gian bắt đầu đã đến hoặc quá hạn
            var bookingsToStart = await _context.Bookings
                .Include(b => b.BookingDetails)
                .Where(b => b.Status == BookingStatus.NotStarted && b.StartDate <= now)
                .ToListAsync();

            // Cập nhật trạng thái thành InProgress
            foreach (var booking in bookingsToStart)
            {
                booking.Status = BookingStatus.InProgress;
            }

            // Kiểm tra các booking đang thực hiện và đã hết hạn
            var bookingsInProgress = await _context.Bookings
                .Include(b => b.BookingDetails)
                .Where(b => b.Status == BookingStatus.InProgress && b.EndDate <= now)
                .ToListAsync();

            // Cập nhật trạng thái thành Completed
            foreach (var booking in bookingsInProgress)
            {
                if (booking.Status != BookingStatus.Canceled)
                {
                    booking.Status = BookingStatus.Completed;
                    foreach (var detail in booking.BookingDetails)
                    {
                        detail.Status = true; // Hoặc sử dụng enum tương ứng nếu có
                    }
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
