using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetSpa.CustomActionFilter;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Manager;
using PetSpa.Repositories.ManagerRepository;

namespace PetSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IManagerRepository managerRepository;
        private readonly ApiResponseService apiResponseService;

        public ManagerController(IMapper mapper, IManagerRepository managerRepository, ApiResponseService apiResponseService)
        {
            this.mapper = mapper;
            this.managerRepository = managerRepository;
            this.apiResponseService = apiResponseService;
        }

        //Get ALl Manager
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
               var manaDomainModels =  await managerRepository.GetAllAsync();
            return Ok(mapper.Map<List<ManagerDTO>>(manaDomainModels));
        }


        //Get Manager By ID
        //Get /api/Manager/{id}
        [HttpGet]
        [Route("{ManaId:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid ManaId)
        {
            var manaDomailModels = await managerRepository.GetByIDAsync(ManaId);
            if (manaDomailModels == null) 
            {
                return apiResponseService.CreateUnauthorizedResponse();
            }

            //Map Domain Model to DTo
             var managerDTO = mapper.Map<ManagerDTO>(manaDomailModels);
            return Ok(apiResponseService.CreateSuccessResponse(managerDTO));
        }

        //Update Manager
        // PUT: /api/Manager/{id}
        [HttpPut]
        [Route("{ManaId:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid ManaId, UpdateManagerRequestDTO updateManagerRequestDTO)
        {
            //Map DTO to Domain Model
            var managerDomainModels = mapper.Map<Manager>(updateManagerRequestDTO);
            managerDomainModels = await managerRepository.UpdateAsync(ManaId, managerDomainModels);

            if (managerDomainModels == null)
            {
                return NotFound();
            }
            //Map Domain Models to DTO
             var manaDTO =  mapper.Map<ManagerDTO>(managerDomainModels);
            return Ok(apiResponseService.CreateSuccessResponse(manaDTO));
        }
    }
}
