﻿using PetSpa.Models.Domain;

namespace PetSpa.Repositories.ServiceRepository
{
    public interface IServiceRepository
    {
        Task<Service> CreateAsync(Service service);
        Task<List<Service>> GetAllAsync();
        Task<Service?> GetByIdAsync(Guid ServiceId);
        Task<Service?> UpdateAsync(Guid ServiceID, Service service);
        Task<Service?> DeleteAsync(Guid ServiceID);

        Task<Service?> AddComboAsync(Guid ServiceID, Guid ComboID);

        Task<Service?> ChangStatus(Guid ServiceID);


    }
}
