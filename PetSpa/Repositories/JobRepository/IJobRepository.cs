using PetSpa.Models.Domain;

namespace PetSpa.Repositories.JobRepository
{
    public interface IJobRepository
    {
        Task<Job> CreateAsync(Job job);
    }
}
