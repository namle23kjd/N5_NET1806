using Microsoft.EntityFrameworkCore;
using PetSpa.Data;
using PetSpa.Models.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            return await dbContext.BookingDetails
                .Include(bd => bd.Combo)
                .Include(bd => bd.Service)
                .Include(bd => bd.Pet)
                .Include(bd => bd.Staff)
                .Include(bd => bd.Booking)
                .ToListAsync();
        }

        public async Task<BookingDetail?> GetByIdAsync(Guid BookingDetailId)
        {
            return await dbContext.BookingDetails
                .Include(bd => bd.Combo)
                .Include(bd => bd.Service)
                .Include(bd => bd.Pet)
                .Include(bd => bd.Staff)
                .Include(bd => bd.Booking)
                .FirstOrDefaultAsync(bd => bd.BookingDetailId == BookingDetailId);
        }

        public async Task<BookingDetail> Update(Guid BookingDetailID, BookingDetail bookingDetail)
        {
            var existingBookingDetail = await dbContext.BookingDetails.FirstOrDefaultAsync(bd => bd.BookingDetailId == BookingDetailID);
            if (existingBookingDetail == null) return null;

            existingBookingDetail.ServiceId = bookingDetail.ServiceId;
            existingBookingDetail.StaffId = bookingDetail.StaffId;
            existingBookingDetail.ComboId = bookingDetail.ComboId;
            existingBookingDetail.BookingId = bookingDetail.BookingId;
            existingBookingDetail.PetId = bookingDetail.PetId;
            existingBookingDetail.ComboType = bookingDetail.ComboType;
            existingBookingDetail.Duration = bookingDetail.Duration;
            existingBookingDetail.Status = bookingDetail.Status;

            await dbContext.SaveChangesAsync();
            return existingBookingDetail;
        }
    }
}
