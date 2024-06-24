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
            existingBooking.StartDate = booking.StartDate;
            existingBooking.EndDate = booking.EndDate;
            existingBooking.CheckAccept = booking.CheckAccept;
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

        public async Task<List<Booking>> GetBookingsByApprovalStatusAsync(bool isApproved)
        {
            return await dbContext.Bookings
                    .Include(b => b.BookingDetails)
                    .Where(b => b.CheckAccept == isApproved)
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
    }
}
