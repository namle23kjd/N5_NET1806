using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetSpa.CustomActionFilter;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Service;
using PetSpa.Repositories.ServiceRepository;

namespace PetSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IServiceRepository serviceRepository;
        private readonly ApiResponseService apiResponseService;

        public ServiceController(IMapper mapper , IServiceRepository serviceRepository, ApiResponseService apiResponseService)
        {
            this.mapper = mapper;
            this.serviceRepository = serviceRepository;
            this.apiResponseService = apiResponseService;
        }
        //Create Service
        //Post: /api/Service
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddServiceRequest addServiceRequest)
        {
            //Map DTO TO domainModel
            var serviceDomainModel = mapper.Map<Service>(addServiceRequest);

            await serviceRepository.CreateAsync(serviceDomainModel);
            var service = mapper.Map<Service>(serviceDomainModel);
            return Ok(apiResponseService.CreateSuccessResponse(service));
        }

        // Get Service
        //Get : /api/Service
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var serviceDomainModel = await serviceRepository.GetAllAsync();
            return Ok(mapper.Map<List<Service>>(serviceDomainModel));
        }

        //Get Services By ID
        // Get : /api/Services/{id}
        [HttpGet]
        [Route("{ServiceId:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid ServiceId)
        {
            var serviceDomainModel = await serviceRepository.GetByIdAsync(ServiceId);

            if(serviceDomainModel == null)
            {
                return apiResponseService.CreateUnauthorizedResponse();
            }
            var service = mapper.Map<ServiceDTO>(serviceDomainModel);
            return Ok(apiResponseService.CreateSuccessResponse(service));
        }

        //Update Service By ServiceId
        //Put: /api/ServiceID/
        [HttpPut]
        [Route("{ServiceId:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid ServiceId , UpdateServiceRequestDTO updateServiceRequestDTO)
        {
            //Map DTO DomainModel
            var serviceDomainModel = mapper.Map<Service>(updateServiceRequestDTO);

            serviceDomainModel = await serviceRepository.UpdateAsync(ServiceId, serviceDomainModel);
            if(serviceDomainModel == null) { return null; }
            return Ok(mapper.Map<ServiceDTO>(serviceDomainModel));
        }


    }
}
