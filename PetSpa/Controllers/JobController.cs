using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Job;
using PetSpa.Repositories.JobRepository;

namespace PetSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IJobRepository jobRepository;

        public JobController(IMapper mapper, IJobRepository jobRepository)
        {
            this.mapper = mapper;
            this.jobRepository = jobRepository;
        }

        //Create Jobs
        //Post: /api/Jobs
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddJobRequestDTO addJobRequestDTO)
        {
            //Map DTO to DomainModel
            var jobDomainModels = mapper.Map<Job>(addJobRequestDTO);
            await jobRepository.CreateAsync(jobDomainModels);

            //Map Domain model tp DTO
            return Ok(mapper.Map<JobDTO>(jobDomainModels));
        }

    }
}
