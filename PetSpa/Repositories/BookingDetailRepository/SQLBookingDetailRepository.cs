using Microsoft.EntityFrameworkCore;
using PetSpa.Data;
using PetSpa.Models.Domain;

namespace PetSpa.Repositories.BookingDetailRepository
{
    public class SQLBookingDetailRepository : IBookingDetailsRepository
    {
        private readonly PetSpaContext dbContext;

        public SQLBookingDetailRepository(PetSpaContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<BookingDetail> CreateAsync(BookingDetail bookingDetail)
        {
            await dbContext.BookingDetails.AddAsync(bookingDetail);
            await dbContext.SaveChangesAsync();
            return bookingDetail;
        }

        public async Task<List<BookingDetail>> GetAllAsync()
        {
            return await dbContext.BookingDetails.Include("Combo").Include("Job").Include("Pet").Include("Service").Include("Staff").Include("Booking").ToListAsync();
        }

        public async Task<BookingDetail?> GetByIdAsync(Guid BookingDetailId)
        {
            return await dbContext.BookingDetails.Include("Combo").Include("Job").Include("Pet").Include("Service").Include("Staff").Include("Booking").FirstOrDefaultAsync(x => x.BookingDetailId == BookingDetailId);
        }

        public async Task<BookingDetail> Update(Guid BookingDetailID, BookingDetail bookingDetail)
        {
            var exsitBookingDetail = await dbContext.BookingDetails.FirstOrDefaultAsync(x => x.BookingDetailId == BookingDetailID);
            if (exsitBookingDetail != null) return null;

            exsitBookingDetail.ServiceId = bookingDetail.ServiceId;
            exsitBookingDetail.StaffId = bookingDetail.StaffId;
            exsitBookingDetail.ComboId = bookingDetail.ComboId;
            exsitBookingDetail.BookingId = bookingDetail.BookingId;
            exsitBookingDetail.PetId = bookingDetail.PetId;
            exsitBookingDetail.ComboType = bookingDetail.ComboType;

            await dbContext.SaveChangesAsync();
            return exsitBookingDetail;

        }
    }
}
