using Microsoft.EntityFrameworkCore;
using PetSpa.Data;
using PetSpa.Models.Domain;

namespace PetSpa.Repositories.ServiceRepository
{
    public class SQLServiceRepository : IServiceRepository
    {
        private readonly PetSpaContext dbContext;

        public SQLServiceRepository(PetSpaContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Service> CreateAsync(Service service)
        {
            await dbContext.Services.AddAsync(service);
            await dbContext.SaveChangesAsync();
            return service;
        }

        public async Task<List<Service>> GetAllAsync()
        {
            return await dbContext.Services
                .Include(s => s.BookingDetails)
                .Include(s => s.Combo)
                .Where(s => s.Status == true) // Only get services with Status true
                .ToListAsync();
        }

        public async Task<Service?> GetByIdAsync(Guid ServiceId)
        {
            return await dbContext.Services
                .Include(s => s.BookingDetails)
                .Include(s => s.Combo)
                .FirstOrDefaultAsync(x => x.ServiceId == ServiceId && x.Status == true); // Only get if Status true
        }

        public async Task<Service?> UpdateAsync(Guid ServiceID, Service service)
        {
            var existService = await dbContext.Services.FirstOrDefaultAsync(x => x.ServiceId == ServiceID);

            if (existService == null) { return null; }

            existService.ServiceName = service.ServiceName;
            existService.ServiceDescription = service.ServiceDescription;
            existService.ServiceImage = service.ServiceImage;
            existService.Status = service.Status;
            existService.Duration = service.Duration;
            existService.ComboId = service.ComboId;

            await dbContext.SaveChangesAsync();
            return existService;
        }

        public async Task<Service?> DeleteAsync(Guid ServiceID)
        {
            var existService = await dbContext.Services.FirstOrDefaultAsync(x => x.ServiceId == ServiceID);
            if (existService == null) { return null; }

            // Thay đổi trạng thái từ true thành false
            existService.Status = false;
            await dbContext.SaveChangesAsync();
            return existService;
        }
    }
}
