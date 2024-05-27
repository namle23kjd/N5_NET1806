using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO;
using PetSpa.Repositories;

namespace PetSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IManagerRepository managerRepository;

        public ManagerController(IMapper mapper, IManagerRepository managerRepository)
        {
            this.mapper = mapper;
            this.managerRepository = managerRepository;
        }


        //Create Manager
        //Post: /api/Manager
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddManagerRequestDTO addManagerRequestDTO)
        {
            // Map DTO to DomainModel
            var managerDomainModel = mapper.Map<Manager>(addManagerRequestDTO);
            await managerRepository.CreateAsync(managerDomainModel);

            //Map Domainl model to DTO
            return Ok(mapper.Map<ManagerDTO>(managerDomainModel));

        }

        //Get ALl Manager
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var managerDomainModels = await managerRepository.GetAllAsync();

            //Map Domain Model to DTO
            return Ok(mapper.Map<List<ManagerDTO>>(managerDomainModels));
        }


        //Get Manager By ID
        //Get /api/Manager/{id}
        [HttpGet]
        [Route("{ManaId:int}")]
        public async Task<IActionResult> GetById([FromRoute] int ManaId)
        {
            var managerDomainModels = await managerRepository.GetByIDAsync(ManaId);
            if (managerDomainModels == null)
            {
                return NotFound();
            }
            //Map Domain Models DTO
            return Ok(mapper.Map<ManagerDTO>(managerDomainModels));
        }

        //Update Manager
        // PUT: /api/Manager/{id}
        [HttpPut]
        [Route("{ManaId:int}")]
        public async Task<IActionResult> Update([FromRoute] int ManaId, UpdateManagerRequestDTO updateManagerRequestDTO)
        {
            //Map DTO to Domain Model
            var managerDomainModels = mapper.Map<Manager>(updateManagerRequestDTO);
            managerDomainModels = await managerRepository.UpdateAsync(ManaId, managerDomainModels);

            if (managerDomainModels == null)
            {
                return NotFound();
            }
            //Map Domain Models to DTO
            return Ok(mapper.Map<ManagerDTO>(managerDomainModels));
        }
    }
}
