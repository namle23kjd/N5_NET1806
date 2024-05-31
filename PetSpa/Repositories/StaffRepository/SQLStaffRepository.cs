using Microsoft.EntityFrameworkCore;
using PetSpa.Data;
using PetSpa.Models.Domain;

namespace PetSpa.Repositories.StaffRepository
{
    public class SQLStaffRepository : IStaffRepository

    {
        private readonly PetSpaContext dbContext;

        public SQLStaffRepository(PetSpaContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Staff> CreateAsync(Staff staff)
        {
            await dbContext.Staff.AddAsync(staff);
            await dbContext.SaveChangesAsync();
            return staff;
        }


        public async Task<List<Staff>> GetALlAsync()
        {
            return await dbContext.Staff.ToListAsync();
        }

        public async Task<Staff?> GetByIdAsync(Guid StaffID)
        {
            return await dbContext.Staff.
                                 FirstOrDefaultAsync(x => x.StaffId == StaffID);
        }

        public async Task<Staff?> UpdateAsync(Guid StaffId, Staff staff)
        {
            var exsitingWalk = await dbContext.Staff.FirstOrDefaultAsync(x => x.StaffId == StaffId);
            if (exsitingWalk == null)
            {
                return null;
            }
            exsitingWalk.FullName = exsitingWalk.FullName;
            exsitingWalk.Gender = exsitingWalk.Gender;
            exsitingWalk.AccId = exsitingWalk.AccId;
            exsitingWalk.Acc = exsitingWalk.Acc;
            exsitingWalk.Bookings = exsitingWalk.Bookings;
            exsitingWalk.BookingDetails = exsitingWalk.BookingDetails;
            exsitingWalk.Jobs = exsitingWalk.Jobs;

            await dbContext.SaveChangesAsync();
            return exsitingWalk;
        }
    }
}
