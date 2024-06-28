using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Booking;

namespace PetSpa.Repositories.BookingRepository
{
    public interface IBookingRepository
    {
        Task<Booking> CreateAsync(Booking booking);
        Task<List<Booking>> GetAllAsync();
        Task<Booking?> GetByIdAsync(Guid BookingId);
        Task<Booking?> UpdateAsync(Guid BookingId, Booking booking);
        Task<bool> IsScheduleTakenAsync(DateTime bookingSchedule);
        Task<Manager> GetManagerWithLeastBookingsAsync();
        Task<List<Booking>> GetCompletedBookingsAsync();
        Task<List<Booking>> GetBookingsByStatusAsync(BookingStatus status);
        Task<List<Booking>> GetBookingsByApprovalStatusAsync(bool isApproved);
        List<Staff> GetAvailableStaffsForStartTime(DateTime startTime, DateTime endTime, Guid? staffId = null);
        (List<Staff>, string) GetAvailableStaffs(DateTime startTime, DateTime endTime, int? periodMonths = null, Guid? staffId = null);
        Task<Booking?> GetCurrentBookingForStaffAsync(Guid staffId);
        Task<List<Booking>> GetBookingHistoryForStaffAsync(Guid staffId);

        Task<decimal> GetAllToTalForMonthAsync(DateTime? startDate);
        Task<decimal> GetAllToTalAsync();
         Task<Booking?> UpdateFeedbackAsync(Guid BookingId, Booking booking);

        Task<List<DailyRevenueDTO>> GetDailyRevenueForCurrentMonthAsync(DateTime? startDate);
    }
}