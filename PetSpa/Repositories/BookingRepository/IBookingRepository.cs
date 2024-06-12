using PetSpa.Models.Domain;

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
        List<Staff> GetAvailableStaffsForStartTime(DateTime startTime, DateTime endTime);
    }
}