using PetSpa.Data;
using PetSpa.Models.Domain;

namespace PetSpa.Repositories.JobRepository
{
    public class SQLJobRepository : IJobRepository
    {
        private readonly PetSpaContext dbContext;

        public SQLJobRepository(PetSpaContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Job> CreateAsync(Job job)
        {
            await dbContext.Jobs.AddAsync(job);
            await dbContext.SaveChangesAsync();
            return job;
        }

    }
}
