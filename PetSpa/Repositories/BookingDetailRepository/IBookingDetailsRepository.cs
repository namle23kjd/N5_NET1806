using PetSpa.Models.Domain;

namespace PetSpa.Repositories.BookingDetailRepository
{
    public interface IBookingDetailsRepository
    {
        Task<BookingDetail> CreateAsync(BookingDetail bookingDetail);
        Task<List<BookingDetail>> GetAllAsync();
        Task<BookingDetail?> GetByIdAsync(Guid BookingDetailId);
        Task<BookingDetail> Update(Guid BookingDetailID, BookingDetail bookingDetail);
    }
}
