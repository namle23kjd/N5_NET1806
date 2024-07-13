using Microsoft.EntityFrameworkCore;
using PetSpa.Data;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Booking;
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
                .AsNoTracking()
                .Include(b => b.BookingDetails)
                .ThenInclude(bd => bd.Combo)
                .Include(b => b.BookingDetails)
                .ThenInclude(bd => bd.Service)
                .Include(b => b.Customer)
                .Include(b => b.Manager)
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
                .FirstOrDefaultAsync(b => b.BookingId == BookingId);
        }

        public async Task<Booking?> UpdateAsync(Guid BookingId, Booking booking)
        {
            var existingBooking = await dbContext.Bookings.FirstOrDefaultAsync(b => b.BookingId == BookingId);
            if (existingBooking == null) return null;

            existingBooking.Status = booking.Status;
            existingBooking.BookingSchedule = booking.BookingSchedule;
            existingBooking.TotalAmount = booking.TotalAmount;
            existingBooking.Feedback = booking.Feedback;
            existingBooking.Feedback = booking.Feedback;
            existingBooking.EndDate = booking.EndDate;
            existingBooking.CheckAccept = booking.CheckAccept;
            existingBooking.CheckAccept = booking.CheckAccept;

            await dbContext.SaveChangesAsync();
            return existingBooking;
        }
        public async Task<Booking?> UpdateFeedbackAsync(Guid BookingId, Booking booking)
        {
            var existingBooking = await dbContext.Bookings.FirstOrDefaultAsync(b => b.BookingId == BookingId);
            if (existingBooking == null) return null;
   
            existingBooking.Feedback = booking.Feedback;
           

            await dbContext.SaveChangesAsync();
            return existingBooking;
        }


        public async Task<bool> IsScheduleTakenAsync(DateTime bookingSchedule)
        {
            return await dbContext.Bookings.AnyAsync(b => b.BookingSchedule == bookingSchedule && b.Status == BookingStatus.InProgress);

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
                .Where(b => b.Status == BookingStatus.Completed)
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

        public (List<Staff>, string) GetAvailableStaffs(DateTime startTime, DateTime endTime, int? periodMonths = null, Guid? staffId = null)
        {
            if (periodMonths.HasValue)
            {
                if (periodMonths != 3 && periodMonths != 6 && periodMonths != 9)
                {
                    throw new ArgumentException("Invalid period. Please choose 3, 6, or 9 months.");
                }

                var availableStaffs = new List<Staff>();
                var currentStartTime = startTime;

                for (int i = 0; i < periodMonths; i++)
                {
                    var currentEndTime = currentStartTime.AddMonths(1);

                    if (staffId.HasValue)
                    {
                        var staff = dbContext.Staff
                            .Include(s => s.BookingDetails)
                            .ThenInclude(bd => bd.Booking)
                            .Where(s => s.StaffId == staffId.Value && !s.BookingDetails.Any(bd =>
                                (currentStartTime < bd.Booking.EndDate && currentEndTime > bd.Booking.StartDate)
                            ))
                            .ToList();

                        if (!staff.Any())
                        {
                            return (null, $"Time conflict for the month starting from {currentStartTime:dd/MM/yyyy HH:mm}");
                        }

                        availableStaffs.AddRange(staff);
                    }
                    else
                    {
                        var staffList = dbContext.Staff
                            .Include(s => s.BookingDetails)
                            .ThenInclude(bd => bd.Booking)
                            .Where(s => !s.BookingDetails.Any(bd =>
                                (currentStartTime < bd.Booking.EndDate && currentEndTime > bd.Booking.StartDate)
                            ))
                            .ToList();

                        if (!staffList.Any())
                        {
                            return (null, $"Thời gian bị trùng cho tháng bắt đầu từ {currentStartTime:dd/MM/yyyy HH:mm}");
                        }

                        availableStaffs.AddRange(staffList);
                    }

                    currentStartTime = currentStartTime.AddMonths(1);
                }

                return (availableStaffs.Distinct().ToList(), null);
            }
            else
            {
                if (staffId.HasValue)
                {
                    var staff = dbContext.Staff
                        .Include(s => s.BookingDetails)
                        .ThenInclude(bd => bd.Booking)
                        .Where(s => s.StaffId == staffId.Value && !s.BookingDetails.Any(bd =>
                            (startTime < bd.Booking.EndDate && endTime > bd.Booking.StartDate)
                        ))
                        .ToList();

                    return (staff, null);
                }

                var allStaff = dbContext.Staff
                    .Include(s => s.BookingDetails)
                    .ThenInclude(bd => bd.Booking)
                    .Where(s => !s.BookingDetails.Any(bd =>
                        (startTime < bd.Booking.EndDate && endTime > bd.Booking.StartDate)
                    ))
                    .ToList();

                return (allStaff, null);
            }
        }

        public async Task<List<Booking>> GetBookingsByStatusAsync(BookingStatus status)
        {
            return await dbContext.Bookings
                    .Include(b => b.BookingDetails)
                    .Where(b => b.Status == status)
                    .ToListAsync();
        }

        public async Task<List<Booking>> GetBookingsByApprovalStatusAsync(CheckAccpectStatus checkAcceptStatus)
        {
            return await dbContext.Bookings
                    .Include(b => b.BookingDetails)
                    .Where(b => b.CheckAccept == checkAcceptStatus)
                    .ToListAsync();
        }

        public async Task<Booking?> GetCurrentBookingForStaffAsync(Guid staffId)
        {
            return await dbContext.BookingDetails
                         .Include(bd => bd.Booking)
                         .Where(bd => bd.StaffId == staffId && bd.Booking.Status == BookingStatus.InProgress)
                         .Select(bd => bd.Booking)
                         .FirstOrDefaultAsync();
        }

        public async Task<List<Booking>> GetBookingHistoryForStaffAsync(Guid staffId)
        {
            return await dbContext.Bookings
            .Include(b => b.BookingDetails)
                .ThenInclude(bd => bd.Combo)
            .Include(b => b.BookingDetails)
                .ThenInclude(bd => bd.Service)
            .Include(b => b.Manager)
            .Where(b => b.BookingDetails.Any(bd => bd.StaffId == staffId))
            .ToListAsync();
            ;
        }

        public async Task<decimal> GetAllToTalForMonthAsync(DateTime? startDate)
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            DateTime effectiveStartDate = startDate.HasValue && startDate.Value >= startOfMonth && startDate.Value <= now
                ? startDate.Value
                : now;

            var totalAmount = await dbContext.Payments
                .Where(p => p.CreatedDate >= effectiveStartDate && p.CreatedDate <= now)
                .Join(dbContext.Bookings,
                    payment => payment.PaymentId,
                    booking => booking.PaymentId,
                    (payment, booking) => booking.TotalAmount ?? 0)
                .SumAsync();

            return totalAmount;
        }

        public async Task<decimal> GetAllToTalAsync()
        {
            return await dbContext.Payments
            .Join(dbContext.Bookings,
                payment => payment.PaymentId,
                booking => booking.PaymentId,
                (payment, booking) => payment.TotalPayment ?? 0)
            .SumAsync();
        }

        public async Task<List<DailyRevenueDTO>> GetDailyRevenueForCurrentMonthAsync(DateTime? startDate)
        {
            var now = DateTime.Now.Date;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            if (!startDate.HasValue)
            {
                // Nếu không có ngày bắt đầu, chỉ trả về doanh thu của ngày hiện tại
                var dailyRevenuesToday = await dbContext.Payments
                    .Where(p => p.CreatedDate.Date == now)
                    .Join(dbContext.Bookings,
                        payment => payment.PaymentId,
                        booking => booking.PaymentId,
                        (payment, booking) => new { payment.CreatedDate, booking.TotalAmount })
                    .GroupBy(p => p.CreatedDate.Date)
                    .Select(g => new DailyRevenueDTO
                    {
                        Date = g.Key,
                        TotalAmount = g.Sum(b => b.TotalAmount ?? 0)
                    })
                    .ToListAsync();

                return dailyRevenuesToday;
            }

            DateTime effectiveStartDate = startDate.Value.Date >= startOfMonth && startDate.Value.Date <= now
                ? startDate.Value.Date
                : startOfMonth;

            var dailyRevenues = await dbContext.Payments
                .Where(p => p.CreatedDate >= effectiveStartDate && p.CreatedDate < now.AddDays(1))
                .Join(dbContext.Bookings,
                    payment => payment.PaymentId,
                    booking => booking.PaymentId,
                    (payment, booking) => new { payment.CreatedDate, booking.TotalAmount })
                .GroupBy(p => p.CreatedDate.Date)
                .Select(g => new DailyRevenueDTO
                {
                    Date = g.Key,
                    TotalAmount = g.Sum(b => b.TotalAmount ?? 0)
                })
                .ToListAsync();

            return dailyRevenues;
        }

        public async Task<List<Booking>> GetBookingsByCheckAcceptAsync(CheckAccpectStatus checkAccept)
        {
                    return await dbContext.Bookings
               .Include(b => b.Customer)
               .Include(b => b.BookingDetails)
                   .ThenInclude(bd => bd.Service)
               .Include(b => b.BookingDetails)
                   .ThenInclude(bd => bd.Pet)
               .Include(b => b.BookingDetails)
                   .ThenInclude(bd => bd.Staff)
               .Where(b => b.CheckAccept == checkAccept)
               .ToListAsync();
        }

        public async Task<List<Booking>> GetDeniedBookingsByStaffIdAsync(Guid staffId)
        {
            return await dbContext.Bookings
                .Include(b => b.Customer)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.Service)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.Pet)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.Staff)
                .Where(b => b.BookingDetails.Any(bd => bd.StaffId == staffId) && b.CheckAccept == CheckAccpectStatus.Deny)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate)
        {
            var totalAmount = await dbContext.Payments
            .Where(p => p.CreatedDate >= startDate && p.CreatedDate <= endDate)
            .SumAsync(p => p.TotalPayment ?? 0m);

            return totalAmount;
        }

        public async Task<List<Booking>> GetBookingsByStatus2Async(BookingStatus status)
        {
            return await dbContext.Bookings
        .Include(b => b.Customer)
        .Include(b => b.BookingDetails)
            .ThenInclude(bd => bd.Service)
        .Include(b => b.BookingDetails)
            .ThenInclude(bd => bd.Pet)
        .Include(b => b.BookingDetails)
            .ThenInclude(bd => bd.Staff)
        .Where(b => b.Status == status)
        .ToListAsync();
        }
    }
}
